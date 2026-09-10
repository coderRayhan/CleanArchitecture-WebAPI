using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Application.Common.Abstractions.Contracts;
using Application.Common.Abstractions.Idempotency;
using Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Behaviours;

public class IdempotencyBehaviour<TRequest, TResponse>(IIdempotencyStore store) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IIdempotentCommand
    where TResponse : Result
{
    private static readonly TimeSpan Ttl = TimeSpan.FromHours(24);

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var key = request.IdempotencyKey;
        var requestHash = ComputeHash(request);
        
        var (checkResult, record) = await store.TryBeginAsync(key, requestHash, Ttl, cancellationToken);

        switch (checkResult)
        {
            case IdempotencyCheckResult.Completed:
                return DeserializeResult<TResponse>(record!);
            
            case IdempotencyCheckResult.InProgress:
                return (TResponse)(object)Result.Failure(new Error("IDEMPOTENCY_IN_PROGRESS",
                    "Request is already being processed.", ErrorType.Failure));
            
            case IdempotencyCheckResult.KeyReusedWithDifferentPayload:
                return (TResponse)(object)Result.Failure(new Error("IDEMPOTENCY_KEY_CONFLICT",
                    "Idempotency-Key was reused with a different payload.", ErrorType.Conflict));
        }

        TResponse response;
        try
        {
            response = await next();
        }
        catch (Exception e)
        {
            await store.ReleaseAsync(key, cancellationToken);
            throw;
        }
        
        await store.CompleteAsync(key, GetStatusCode(response), JsonSerializer.Serialize(response), cancellationToken);

        return response;
    }
    
    private static string ComputeHash(TRequest request) => 
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request))));
    
    private static int GetStatusCode(Result response) =>
        response.Error.ErrorType switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError

        };
    
    private static T DeserializeResult<T>(IdempotencyRecord record) =>
        JsonSerializer.Deserialize<T>(record.ResponseBody!)!;
}
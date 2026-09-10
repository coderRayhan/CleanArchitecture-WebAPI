using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Application.Common.Abstractions.Contracts;
using Application.Common.Abstractions.Idempotency;
using Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Behaviours;

// public class IdempotencyBehaviour<TRequest, TResponse>(IIdempotencyStore store) : IPipelineBehavior<TRequest, TResponse>
//     where TRequest : IIdempotentCommand
//     where TResponse : Result
// {
//     private static readonly TimeSpan Ttl = TimeSpan.FromHours(24);
//
//     public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
//     {
//         var key = request.IdempotencyKey;
//         var requestHash = 
//     }
//     
//     private static string ComputeHash(TRequest request) => 
//         Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request))));
//     
//     private static int GetStatusCode(TResponse response) => 
//         response.IsSuccess ? StatusCodes.Status200OK : response.Error.ErrorType
// }
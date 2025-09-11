using Application.Common.Abstractions.Caching;
using Application.Common.Abstractions.Contracts;
using Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Common.Behaviours;
internal sealed class LazyCachingBehaviour<TRequest, TResponse>(
    ILazyCacheService cacheService,
    ILogger<LazyCachingBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableQuery
    where TResponse : Result
{
    private static readonly JsonSerializerOptions serializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        IncludeFields = true
    };
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {

        if (request.AllowCache ?? true)
        {
            string? cachedJson = await cacheService.GetStringAsync(request.CacheKey, cancellationToken);
            if(cachedJson is not null)
            {
                logger.LogInformation("Cache hit for {RequestName} with key {CacheKey}",
                    typeof(TRequest).Name, request.CacheKey);

                return JsonSerializer.Deserialize<TResponse>(cachedJson, serializerOptions)!;
            }
        }

        TResponse result = await next();

        if (result.IsSuccess)
        {
            string json = JsonSerializer.Serialize(result, serializerOptions);

            await cacheService.SetStringAsync(
                request.CacheKey,
                json,
                request.Expiration,
                cancellationToken).ConfigureAwait(false);

            logger.LogInformation("Added to cache: {RequestName} with key {CacheKey}",
                typeof(TRequest).Name,
                request.CacheKey);
        }

        return result;
    }
}

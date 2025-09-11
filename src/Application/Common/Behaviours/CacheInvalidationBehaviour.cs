using Application.Common.Abstractions.Caching;
using Application.Common.Abstractions.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours;
internal sealed class CacheInvalidationBehaviour<TRequest, TResponse>(
    ILogger<CacheInvalidationBehaviour<TRequest, TResponse>> logger,
    IDistributedCacheService cacheService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheInvalidatorCommand
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var result = await next().ConfigureAwait(false);

        if (request.CacheKeys is not null && request.CacheKeys?.Length > 0)
        {
            var tasks = request.CacheKeys.Select(async cacheKey =>
            {
                await cacheService.RemoveByPrefixAsync(cacheKey, cancellationToken);
                logger.LogInformation("Cache value of {CacheKey} expired with {@Request}", cacheKey, request);
            });

            await Task.WhenAll(tasks);
        }

        return result;
    }
}

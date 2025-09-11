using Application.Common.Abstractions.Caching;
using LazyCache;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Caching;
internal sealed class LazyCacheService(
    IAppCache cache,
    IOptions<CacheOptions> cacheOptions,
    ILogger<LazyCacheService> logger)
    : ILazyCacheService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        return await cache.GetAsync<T>(key);
    }

    public Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RemobeByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SetStringAsync(string key, string value, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

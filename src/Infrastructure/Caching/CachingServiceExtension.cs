using Application.Common.Abstractions.Caching;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure.Caching;
public static class CachingServiceExtension
{
    private const string RedisConnection = nameof(RedisConnection);

    public static IServiceCollection AddCachingService(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureOptions<CacheOptionsSetup>();

        #region Distributed Cache
        var redisConString = configuration.GetConnectionString(RedisConnection);
        Guard.Against.Null(redisConString, message: "Connection string 'RedisCache' not found");

        services.AddSingleton(ConnectionMultiplexer.Connect(redisConString));
        services.AddStackExchangeRedisCache(options => options.Configuration = redisConString);
        services.AddDistributedMemoryCache();
        services.AddSingleton<IDistributedCacheService, DistributedCacheService>();
        #endregion

        #region In-Memory Cache
        services.AddMemoryCache();
        services.AddSingleton<IInMemoryCacheService, InMemoryCacheService>();
        #endregion

        #region Lazy Cache
        services.AddLazyCache();
        services.AddSingleton<ILazyCacheService, LazyCacheService>();
        #endregion

        return services;
    }
}

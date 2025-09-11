using Application.Common.Abstractions;
using Application.Common.Abstractions.Caching;
using Application.Common.Abstractions.Identity;
using Ardalis.GuardClauses;
using Domain.Constants;
using Infrastructure.Caching;
using Infrastructure.Configurations;
using Infrastructure.Identity;
using Infrastructure.Identity.Model;
using Infrastructure.Identity.OptionsSetup;
using Infrastructure.Identity.Permissions;
using Infrastructure.Identity.Services;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection;
public static class DependencyInjection
{
    private const string DefaultConnection = nameof(DefaultConnection);
    private const string IdentityConnection = nameof(IdentityConnection);
    private const string RedisConnection = nameof(RedisConnection);
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {

        PersistenceServiceExtension.AddPersistenceService(services, configuration);
        CachingServiceExtension.AddCachingService(services, configuration);
        IdentityServiceExtension.AddIdentityService(services, configuration);
        return services;
    }

}
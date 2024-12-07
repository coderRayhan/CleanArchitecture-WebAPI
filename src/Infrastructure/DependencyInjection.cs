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

    private const string DATABASE_SETTINGS_KEY = "DatabaseSettings";
    private const string USE_IN_MEMORY_DATABASE_KEY = "UseInMemoryDatabase";
    private const string IN_MEMORY_DATABASE_NAME = "BlazorDashboardDb";
    private const string NPGSQL_ENABLE_LEGACY_TIMESTAMP_BEHAVIOR = "Npgsql.EnableLegacyTimestampBehavior";
    private const string POSTGRES_MIGRATION_ASSEMBLY = "";
    private const string MSSQL_MIGRATION_ASSEMBLY = "";
    //private const string DefaultConnection = nameof(DefaultConnection);
    //private const string IdentityConnection = nameof(IdentityConnection);
    //private const string RedisConnection = nameof(RedisConnection);
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        //var dbConnectionString = configuration.GetConnectionString(DefaultConnection);
        //var identityConnectionString = configuration.GetConnectionString(IdentityConnection);
        //var redisConnectionString = configuration.GetConnectionString(RedisConnection);

        //Guard.Against.Null(dbConnectionString, message: $"Connection string '{nameof(DefaultConnection)}' not found");
        //Guard.Against.Null(identityConnectionString, message: $"Connection string '{nameof(IdentityConnection)}' not found");
        //Guard.Against.Null(redisConnectionString, message: $"Connection string '{nameof(RedisConnection)}' not found");

        var provider = services.BuildServiceProvider();
        var databaseSettings = provider.GetRequiredService<IOptions<DatabaseSettings>>().Value;

        services.AddSettings(configuration)
        .AddPersistence(configuration, databaseSettings)
        .AddRedisCache(databaseSettings)
        .AddIdentity()
        .AddAuthenticationAuthorization()
        .AddCaching();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration, DatabaseSettings databaseSettings)
    {
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

        if (configuration.GetValue<bool>(USE_IN_MEMORY_DATABASE_KEY))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(IN_MEMORY_DATABASE_NAME);
                options.EnableSensitiveDataLogging();
            });

            services.AddDbContext<IdentityContext>(op => op.UseInMemoryDatabase(USE_IN_MEMORY_DATABASE_KEY));
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>((p, m) =>
            {
                //var databaseSettings = p.GetRequiredService<IOptions<DatabaseSettings>>().Value;
                m.AddInterceptors(p.GetService<ISaveChangesInterceptor>());
                m.UseDatabase(databaseSettings.DBProvider, databaseSettings.ConnectionString);
            });

            services.AddDbContext<IdentityContext>((op, m) =>
            {
                //var databaseSettings = op.GetRequiredService<IOptions<DatabaseSettings>>().Value;
                m.UseDatabase(databaseSettings.DBProvider, databaseSettings.ConnectionString);
            });
        }

        //services.AddDbContext<ApplicationDbContext>((sp, options) =>
        //{
        //    options.AddInterceptors(sp.GetRequiredService<ISaveChangesInterceptor>());

        //    options.UseSqlServer(appDbConString);
        //});

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IdentityContextInitialiser>();

        //services.AddDbContext<IdentityContext>(op => op.UseSqlServer(identityDbConString));

        return services;
    }

    private static DbContextOptionsBuilder UseDatabase(this DbContextOptionsBuilder builder, string dbProvider, string connectionString)
    {
        switch (dbProvider.ToLowerInvariant())
        {
            case "NpgSql":
                AppContext.SetSwitch(NPGSQL_ENABLE_LEGACY_TIMESTAMP_BEHAVIOR, true);
                return builder.UseNpgsql(connectionString, e => e.MigrationsAssembly(POSTGRES_MIGRATION_ASSEMBLY));
            case "Mssql":
                return builder.UseSqlServer(connectionString, e => e.MigrationsAssembly(MSSQL_MIGRATION_ASSEMBLY));
            default:
                throw new InvalidOperationException($"DB Provider {dbProvider} is not supported.");
        }
    }

    private static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseSettings>(configuration.GetSection(DATABASE_SETTINGS_KEY)).AddSingleton(s => s.GetRequiredService<IOptions<DatabaseSettings>>());

        return services;
    }

    private static IServiceCollection AddRedisCache(this IServiceCollection services, DatabaseSettings databaseSettings)
    {
        services.AddSingleton(ConnectionMultiplexer.Connect(databaseSettings.RedisConnection));
        services.AddStackExchangeRedisCache(op => op.Configuration = databaseSettings.RedisConnection);

        return services;
    }

    private static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<IdentityContext>()
            .AddApiEndpoints();

        services.AddTransient<IIdentityService, IdentityService>();

        services.AddTransient<IIdentityRoleService, IdentityRoleService>();

        services.AddTransient<IAuthService, AuthService>();

        services.AddTransient<ITokenProviderService, TokenProviderService>();

        return services;
    }
    private static IServiceCollection AddAuthenticationAuthorization(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer();

        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.AddAuthorizationBuilder();

        services.AddSingleton(TimeProvider.System);

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator));
        });

        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

        return services;
    }

    private static IServiceCollection AddCaching(this IServiceCollection services)
    {
        services.AddLazyCache();
        services.ConfigureOptions<CacheOptionsSetup>();
        services.AddSingleton<IInMemoryCacheService, InMemoryCacheService>();
        services.AddSingleton<IDistributedCacheService, DistributedCacheService>();

        return services;
    }
}

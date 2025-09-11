using Application.Common.Abstractions.Identity;
using Ardalis.GuardClauses;
using Domain.Constants;
using Infrastructure.Identity.Model;
using Infrastructure.Identity.OptionsSetup;
using Infrastructure.Identity.Permissions;
using Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Identity;
public static class IdentityServiceExtension
{
    public const string IdentityConnection = nameof(IdentityConnection);
    public static IServiceCollection AddIdentityService(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(IdentityConnection);
        Guard.Against.Null(connectionString, message: $"Connection string {nameof(IdentityConnection)} not found");

        services.AddDbContext<IdentityContext>(options => options.UseSqlServer(connectionString));

        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<IdentityContext>()
            .AddApiEndpoints();

        services.AddTransient<IIdentityService, IdentityService>();

        services.AddTransient<IIdentityRoleService, IdentityRoleService>();

        services.AddTransient<IAuthService, AuthService>();

        services.AddTransient<ITokenProviderService, TokenProviderService>();


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
}

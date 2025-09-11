using API.Middlewares;
using API.Services;
using Application.Common.Abstractions.Identity;
using Microsoft.AspNetCore.Mvc;
using NSwag;
using NSwag.Generation.Processors.Security;
using System.Text.Json;
using ZymLabs.NSwag.FluentValidation;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        AddDatabaseDeveloperPageExceptionFilter(services);
        AddJsonConfiguration(services);
        AddScopedServices(services);
        AddExceptionHandlers(services);
        AddHttpContextAccessor(services);
        AddFluentValidationSchemaProcessor(services);
        ConfigureApiBehaviour(services);
        AddOpenApiDocument(services);

        return services;
    }

    private static void AddDatabaseDeveloperPageExceptionFilter(IServiceCollection services)
    {
        services.AddDatabaseDeveloperPageExceptionFilter();
    }

    private static void AddJsonConfiguration(IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        });
    }

    private static void AddScopedServices(IServiceCollection services)
    {
        services.AddScoped<IUser, CurrentUser>();
    }

    private static void AddHttpContextAccessor(IServiceCollection services)
    {
        services.AddHttpContextAccessor();
    }

    private static void AddExceptionHandlers(IServiceCollection services)
    {
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
    }

    private static void AddFluentValidationSchemaProcessor(IServiceCollection services)
    {
        services.AddScoped(provider =>
        {
            var validationRules = provider.GetService<IEnumerable<FluentValidationRule>>();

            var loggerFactory = provider.GetService<ILoggerFactory>();

            return new FluentValidationSchemaProcessor(provider, validationRules, loggerFactory);
        });
    }

    private static void ConfigureApiBehaviour(IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        options.SuppressModelStateInvalidFilter = true);
    }

    private static void AddOpenApiDocument(IServiceCollection services)
    {
        services.AddOpenApiDocument((settings, sp) =>
        {
            settings.Title = "API";

            var fluentValidationSchemaProcessor = sp.CreateScope()
            .ServiceProvider.GetRequiredService<FluentValidationSchemaProcessor>();

            settings.SchemaSettings.SchemaProcessors.Add(fluentValidationSchemaProcessor);

            settings.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "Type into the textbox: Bearer {your JWT token}."
            });

            settings.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
        });
    }
}

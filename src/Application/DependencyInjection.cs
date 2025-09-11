using Application.Common.Behaviours;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), includeInternalTypes: true);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());

            cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));

            cfg.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));

            cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));

            //cfg.AddOpenBehavior(typeof(MemoryCachingBehaviour<,>));

            //cfg.AddOpenBehavior(typeof(QueryCachingBehaviour<,>));

            //cfg.AddOpenBehavior(typeof(CacheInvalidationBehaviour<,>));

            cfg.AddOpenBehavior(typeof(LazyCachingBehaviour<,>));
            cfg.AddOpenBehavior(typeof(LazyCacheInvalidationBehavior<,>));

        });

        services.AddMapster();

        return services;
    }
}

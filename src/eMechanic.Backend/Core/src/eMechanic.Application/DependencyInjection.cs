namespace eMechanic.Application;

using System.Reflection;
using Behaviors;
using Caching;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        services.AddValidatorsFromAssembly(executingAssembly, ServiceLifetime.Transient);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(executingAssembly);
            cfg.AddOpenBehavior(typeof(QueryValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
        });

        services.RegisterCacheConfiguration();
    }

    private static void RegisterCacheConfiguration(this IServiceCollection services)
    {
        var cacheConfig = new CacheConfiguration();

        // cacheConfig.Register(new CacheRule<GetCurrentUserQuery>(
        //     TimeSpan.FromMinutes(5),
        //     q => ECacheKey.GetUserById.ToCacheKeyString()
        // ));

        services.AddSingleton<ICacheConfiguration>(cacheConfig);
    }
}

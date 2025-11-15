namespace eMechanic.Application;

using System.Reflection;
using Abstractions.Storage;
using Behaviors;
using Caching;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Storage.Builders;
using Vehicle.Services;

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

        services.AddServices();
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

    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IVehicleOwnershipService, VehicleOwnershipService>();
        services.AddScoped<IVehicleDocumentPathBuilder, VehicleDocumentPathBuilder>();
    }
}

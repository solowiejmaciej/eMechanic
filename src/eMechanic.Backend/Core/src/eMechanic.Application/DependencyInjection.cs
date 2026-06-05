namespace eMechanic.Application;

using System.Reflection;
using Behaviors;
using Caching;
using Common.Cache;
using eMechanic.Application.Payments.Abstractions;
using eMechanic.Application.Payments.Services;
using eMechanic.Application.Payments.Strategies;
using eMechanic.Application.Repair.PaymentStrategies;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RepairRequest.Services;
using Vehicle.Vehicle.Services;

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
        services.AddScoped<ICacheKeyGenerator, CacheKeyGenerator>();

        var cacheConfig = new CacheConfiguration();

        var executingAssembly = Assembly.GetExecutingAssembly();
        var typesWithCache = executingAssembly.GetTypes()
            .Where(t => t.GetCustomAttributes(typeof(CacheAttribute), false).Length != 0);

        foreach (var type in typesWithCache)
        {
            var attr = (CacheAttribute)type.GetCustomAttributes(typeof(CacheAttribute), false).First();
            var ruleType = typeof(CacheRule<>).MakeGenericType(type);
            var rule = Activator.CreateInstance(ruleType, new object[] { TimeSpan.FromSeconds(attr.DurationSeconds), attr.Scope });
            var registerMethod = typeof(CacheConfiguration).GetMethod("Register")!.MakeGenericMethod(type);
            registerMethod.Invoke(cacheConfig, [rule!]);
        }

        services.AddSingleton<ICacheConfiguration>(cacheConfig);
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IVehicleOwnershipService, VehicleOwnershipService>();
        services.AddScoped<IRepairRequestSummaryService, RepairRequestSummaryService>();
        services.AddScoped<IPaymentOrderProcessor, PaymentOrderProcessor>();

        // Payment strategies — add new types here to support additional payable items
        services.AddScoped<IPaymentInitializationStrategy, RepairPaymentInitializationStrategy>();
        services.AddScoped<IPaymentConfirmationStrategy, RepairPaymentConfirmationStrategy>();
    }
}

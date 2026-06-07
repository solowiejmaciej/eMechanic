namespace eMechanic.Application;

using System.Reflection;
using Behaviors;
using Common.Cache.DependencyInjection;
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
            cfg.AddOpenBehavior(typeof(MediatRPipelineAdapterBehavior<,>));
        });

        services.AddCache(executingAssembly);

        services.AddServices();
    }


    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IVehicleOwnershipService, VehicleOwnershipService>();
        services.AddScoped<IRepairRequestSummaryService, RepairRequestSummaryService>();
        services.AddScoped<IPaymentOrderProcessor, PaymentOrderProcessor>();

        services.AddScoped<IPaymentInitializationStrategy, RepairPaymentInitializationStrategy>();
        services.AddScoped<IPaymentConfirmationStrategy, RepairPaymentConfirmationStrategy>();
    }
}

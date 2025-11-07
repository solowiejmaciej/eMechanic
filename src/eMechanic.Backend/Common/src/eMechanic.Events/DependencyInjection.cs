namespace eMechanic.Events;

using System.Reflection;
using Factories;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services;

public static class ServiceCollectionExtensions
{
    public static void AddEventPublishing(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEventPublisher, EventPublisher>();
        services.AddSingleton<IEventFactory, EventFactory>();
        ConfigureMassTransit(services, configuration, null);
    }

    public static void AddEventConsuming(this IServiceCollection services, IConfiguration configuration,
        Assembly assemblyContainingConsumers)
    {
        var consumerTypes = assemblyContainingConsumers.GetTypes()
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumer<>)))
            .ToArray();

        ConfigureMassTransit(services, configuration, consumerTypes);
    }

    public static void AddEventConsumingAndPublishing(this IServiceCollection services, IConfiguration configuration,
        Assembly assemblyContainingConsumers)
    {
        var consumerTypes = assemblyContainingConsumers.GetTypes()
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumer<>)))
            .ToArray();

        services.AddScoped<IEventPublisher, EventPublisher>();
        ConfigureMassTransit(services, configuration, consumerTypes);
    }

    private static void ConfigureMassTransit(
        IServiceCollection services,
        IConfiguration configuration,
        Type[]? consumerTypes)
    {
        var connectionString = configuration.GetConnectionString("AzureServiceBus");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Unable to get AzureServiceBus ConnectionString");
        }

        var serviceName = Assembly.GetEntryAssembly()?.GetName().Name;

        services.AddMassTransit(x =>
        {
            if (consumerTypes != null)
            {
                foreach (var consumerType in consumerTypes)
                {
                    x.AddConsumer(consumerType);
                }
            }

            x.UsingAzureServiceBus((context, cfg) =>
            {
                cfg.Host(connectionString);

                if (consumerTypes != null)
                {
                    foreach (var consumerType in consumerTypes)
                    {
                        var messageTypeInterface = consumerType.GetInterfaces()
                            .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumer<>));
                        var messageType = messageTypeInterface.GetGenericArguments()[0];

                        var queueName = $"{serviceName}.{messageType.Name}";

                        cfg.ReceiveEndpoint(queueName, e =>
                        {
                            e.ConfigureConsumer(context, consumerType);

                            e.PrefetchCount = 16;
                            e.UseMessageRetry(r => r.Intervals(100, 500, 1000));
                        });
                    }
                }
            });
        });
    }
}

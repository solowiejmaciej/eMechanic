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
                    var consumersByMessageType = consumerTypes
                        .Select(consumerType => new
                        {
                            ConsumerType = consumerType,
                            MessageType = consumerType.GetInterfaces()
                                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumer<>))
                                .GetGenericArguments()[0],
                        })
                        .GroupBy(x => x.MessageType);

                    foreach (var messageTypeGroup in consumersByMessageType)
                    {
                        var queueName = $"{serviceName}.{messageTypeGroup.Key.Name}";

                        cfg.ReceiveEndpoint(queueName, e =>
                        {
                            foreach (var consumer in messageTypeGroup)
                            {
                                e.ConfigureConsumer(context, consumer.ConsumerType);
                            }

                            e.PrefetchCount = 16;
                            e.UseMessageRetry(r => r.Intervals(100, 500, 1000));
                        });
                    }
                }
            });
        });
    }
}

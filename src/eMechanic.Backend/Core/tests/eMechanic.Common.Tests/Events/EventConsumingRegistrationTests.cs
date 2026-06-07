namespace eMechanic.Common.Tests.Events;

using eMechanic.Events;
using eMechanic.Events.Events;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class EventConsumingRegistrationTests
{
    [Fact]
    public async Task AddEventConsuming_ShouldRegisterSingleEndpointPerMessageType_WhenMultipleConsumersHandleSameMessage()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:AzureServiceBus"] =
                    "Endpoint=sb://localhost/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=abc="
            })
            .Build();

        var services = new ServiceCollection();
        services.AddEventConsuming(configuration, typeof(FirstUserCreatedConsumer).Assembly);

        await using var serviceProvider = services.BuildServiceProvider();

        // Act
        var exception = Record.Exception(() => serviceProvider.GetRequiredService<IBus>());

        // Assert
        Assert.Null(exception);
    }

    private sealed class UserCreatedForTestEvent : EventBase;

    private sealed class FirstUserCreatedConsumer : IConsumer<UserCreatedForTestEvent>
    {
        public Task Consume(ConsumeContext<UserCreatedForTestEvent> context) => Task.CompletedTask;
    }

    private sealed class SecondUserCreatedConsumer : IConsumer<UserCreatedForTestEvent>
    {
        public Task Consume(ConsumeContext<UserCreatedForTestEvent> context) => Task.CompletedTask;
    }
}


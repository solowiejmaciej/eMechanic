namespace eMechanic.NotificationService;

using Events.Events.User;
using Events.Services;
using MassTransit;

public class UserCreatedConsumer : IEventConsumer<UserCreatedEvent>
{
    private readonly ILogger<UserCreatedConsumer> _logger;

    public UserCreatedConsumer(ILogger<UserCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var @event = context.Message;
        _logger.LogInformation("UserCreatedEvent was consumed {EventUserId}", @event.UserId);
        await Task.Delay(100);
    }
}

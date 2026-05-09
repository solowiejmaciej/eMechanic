using eMechanic.Events.Events.RepairRequest;
using eMechanic.Events.Services;
using eMechanic.NotificationService.Services.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace eMechanic.NotificationService.Consumers;

public class RepairRequestAcceptedConsumer : IEventConsumer<RepairRequestAcceptedEvent>
{
    private readonly INotificationDispatcher _dispatcher;
    private readonly ILogger<RepairRequestAcceptedConsumer> _logger;

    public RepairRequestAcceptedConsumer(INotificationDispatcher dispatcher, ILogger<RepairRequestAcceptedConsumer> logger)
    {
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RepairRequestAcceptedEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("RepairRequestAcceptedEvent was consumed {EventUserId}", msg.UserId);

        var html = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #eee; padding: 20px;'>
            <h2 style='color: #27ae60;'>Naprawa Zaakceptowana!</h2>
            <p>Warsztat zaakceptował Twoje zgłoszenie o numerze: <b>{msg.RepairRequestId}</b>.</p>
            <p>Możesz teraz spodziewać się wstępnej wyceny lub kontaktu w celu ustalenia terminu podstawienia auta.</p>
            <a href='#' style='background: #27ae60; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Sprawdź szczegóły</a>
        </div>";

        await _dispatcher.DispatchAsync(
            msg.UserId,
            "Twoja naprawa została zaakceptowana!",
            html,
            context.CancellationToken);



        _logger.LogInformation("User has been notified that the request: {RequestId} has been accepted.", msg.RepairRequestId);
    }
}

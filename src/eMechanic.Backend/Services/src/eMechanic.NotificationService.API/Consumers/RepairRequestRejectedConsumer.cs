using eMechanic.Events.Events.RepairRequest;
using eMechanic.Events.Services;
using eMechanic.NotificationService.Services.Abstractions;
using MassTransit;

namespace eMechanic.NotificationService.Consumers;

public class RepairRequestRejectedConsumer : IEventConsumer<RepairRequestRejectedEvent>
{
    private readonly INotificationDispatcher _dispatcher;
    private readonly ILogger<RepairRequestRejectedConsumer> _logger;

    public RepairRequestRejectedConsumer(INotificationDispatcher dispatcher, ILogger<RepairRequestRejectedConsumer> logger)
    {
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RepairRequestRejectedEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("RepairRequestRejectedEvent consumed for request {RequestId}", msg.RepairRequestId);

        var html = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #eee; padding: 20px; border-top: 5px solid #e74c3c;'>
                <h2 style='color: #c0392b;'>Zgłoszenie odrzucone</h2>
                <p>Niestety warsztat nie może zająć się Twoim zgłoszeniem {msg.RepairRequestId} w wybranym terminie lub zakresie.</p>
                <p>Możesz spróbować wysłać zgłoszenie do innego warsztatu w naszej bazie.</p>
            </div>";

        await _dispatcher.DispatchAsync(
            msg.UserId,
            "Status zgłoszenia: Odrzucone",
            html,
            context.CancellationToken);



        _logger.LogInformation("Workshop has been notified that request: {RequestId} has been rejected.", msg.RepairRequestId);

    }
}

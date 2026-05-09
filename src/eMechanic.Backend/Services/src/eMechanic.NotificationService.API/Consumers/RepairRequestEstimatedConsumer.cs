using eMechanic.Events.Events.RepairRequest;
using eMechanic.Events.Services;
using eMechanic.NotificationService.Services.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace eMechanic.NotificationService.Consumers;

public class RepairRequestEstimatedConsumer : IEventConsumer<RepairRequestEstimatedEvent>
{
    private readonly INotificationDispatcher _dispatcher;
    private readonly ILogger<RepairRequestEstimatedConsumer> _logger;

    public RepairRequestEstimatedConsumer(INotificationDispatcher dispatcher, ILogger<RepairRequestEstimatedConsumer> logger)
    {
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RepairRequestEstimatedEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("RepairRequestEstimatedEvent consumed for request {RequestId}", msg.RepairRequestId);

        var html = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #eee; padding: 20px;'>
            <h2 style='color: #f39c12;'>Nowa Wycena Naprawy</h2>
            <p>Mechanik przygotował kosztorys dla Twojego pojazdu.</p>
            <p>Szacowany koszt: <span style='font-size: 18px; color: #e67e22;'><b>{msg.EstimatedCostAmount} {msg.EstimatedCostCurrency}</b></span></p>
            <p>Zaloguj się do panelu eMechanic, aby zaakceptować lub odrzucić wycenę.</p>
        </div>";

        await _dispatcher.DispatchAsync(
            msg.UserId,
            "Nowa wycena naprawy",
            html,
            context.CancellationToken);



        _logger.LogInformation("Wysłano powiadomienie o wycenie dla zgłoszenia {RequestId}", msg.RepairRequestId);
    }
}

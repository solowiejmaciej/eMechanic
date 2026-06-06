using eMechanic.Events.Events.RepairRequest;
using eMechanic.Events.Services;
using eMechanic.NotificationService.Services.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace eMechanic.NotificationService.Consumers;

public class RepairRequestCreatedConsumer : IEventConsumer<RepairRequestCreatedEvent>
{
    private readonly INotificationDispatcher _dispatcher;
    private readonly ILogger<RepairRequestCreatedConsumer> _logger;

    public RepairRequestCreatedConsumer(INotificationDispatcher dispatcher, ILogger<RepairRequestCreatedConsumer> logger)
    {
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RepairRequestCreatedEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("RepairRequestCreatedEvent was consumed {EventUserId}", msg.UserId);

        var html = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #eee; padding: 20px; border-top: 5px solid #3498db;'>
                <h2 style='color: #2980b9;'>Zgłoszenie zostało wysłane!</h2>
                <p>Twoja prośba o naprawę została zarejestrowana w systemie.</p>
                <p><b>Numer zgłoszenia:</b> {msg.RepairRequestId}</p>
                <p>Teraz czekamy na akceptację warsztatu. Poinformujemy Cię o kolejnych krokach!</p>
            </div>";

        await _dispatcher.DispatchAsync(
            msg.UserId,
            "Potwierdzenie zgłoszenia naprawy",
            html,
            context.CancellationToken);

        _logger.LogInformation("Email notification has been sent for the report: {EventUserId}", msg.UserId);
    }
}

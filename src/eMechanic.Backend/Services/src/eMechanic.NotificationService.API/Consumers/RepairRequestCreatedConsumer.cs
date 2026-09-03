using eMechanic.Events.Events.Repair;
using eMechanic.Events.Services;
using eMechanic.NotificationService.Constans;
using eMechanic.NotificationService.Helpers;
using eMechanic.NotificationService.Services.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace eMechanic.NotificationService.Consumers;

public class RepairCreatedConsumer : IEventConsumer<RepairCreatedEvent>
{
    private readonly INotificationDispatcher _dispatcher;
    private readonly ILogger<RepairCreatedConsumer> _logger;

    public RepairCreatedConsumer(INotificationDispatcher dispatcher, ILogger<RepairCreatedConsumer> logger)
    {
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RepairCreatedEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation("{EventName} consumed. [UserId: {UserId}, RepairRequestId: {RepairRequestId}]",
            nameof(RepairCreatedEvent), message.UserId, message.RepairRequestId);

        var title = "Zgłoszenie zostało wysłane!";
        var content = $@"
            <p>Twoja prośba o naprawę została zarejestrowana w systemie.</p>
            <p><b>Numer zgłoszenia:</b> {message.RepairRequestId}</p>
            <p>Teraz czekamy na akceptację warsztatu. Poinformujemy Cię o kolejnych krokach!</p>";

        var emailHtml = EmailTemplateBuilder.Build(title, content);

        await _dispatcher.DispatchAsync(
            message.UserId,
            EmailSubjects.RepairCreated,
            emailHtml,
            context.CancellationToken);

        _logger.LogInformation("Notification dispatched successfully for event {EventName}. [UserId: {UserId}]",
            nameof(RepairCreatedEvent), message.UserId);
    }
}

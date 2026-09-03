using eMechanic.Events.Events.RepairRequest;
using eMechanic.Events.Services;
using eMechanic.NotificationService.Constans;
using eMechanic.NotificationService.Helpers;
using eMechanic.NotificationService.Services.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

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
        var message = context.Message;

        _logger.LogInformation("{EventName} consumed. [UserId: {UserId}, RepairRequestId: {RepairRequestId}]",
            nameof(RepairRequestAcceptedEvent), message.UserId, message.RepairRequestId);

        var title = "Naprawa Zaakceptowana!";
        var content = $@"
            <p>Warsztat zaakceptował Twoje zgłoszenie o numerze: <b>{message.RepairRequestId}</b>.</p>
            <p>Możesz teraz spodziewać się wstępnej wyceny lub kontaktu w celu ustalenia terminu podstawienia auta.</p>
            <p>Zaloguj się do systemu, aby sprawdzić szczegóły.</p>";

        var emailHtml = EmailTemplateBuilder.Build(title, content);

        await _dispatcher.DispatchAsync(
            message.UserId,
            EmailSubjects.RepairRequestAccepted,
            emailHtml,
            context.CancellationToken);

        _logger.LogInformation("Notification dispatched successfully for event {EventName}. [UserId: {UserId}]",
            nameof(RepairRequestAcceptedEvent), message.UserId);
    }
}

using eMechanic.Events.Events.RepairRequest;
using eMechanic.Events.Services;
using eMechanic.NotificationService.Constans;
using eMechanic.NotificationService.Helpers;
using eMechanic.NotificationService.Services.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

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
        var message = context.Message;

        _logger.LogInformation("{EventName} consumed. [UserId: {UserId}, RepairRequestId: {RepairRequestId}]",
            nameof(RepairRequestRejectedEvent), message.UserId, message.RepairRequestId);

        var title = "Zgłoszenie odrzucone";
        var content = $@"
            <p>Niestety warsztat nie może zająć się Twoim zgłoszeniem o numerze <b>{message.RepairRequestId}</b> w wybranym terminie lub zakresie.</p>
            <p>Możesz spróbować wysłać zgłoszenie do innego warsztatu w naszej bazie.</p>";

        var emailHtml = EmailTemplateBuilder.Build(title, content);

        await _dispatcher.DispatchAsync(
            message.UserId,
            EmailSubjects.RepairRequestRejected,
            emailHtml,
            context.CancellationToken);

        _logger.LogInformation("Notification dispatched successfully for event {EventName}. [UserId: {UserId}]",
            nameof(RepairRequestRejectedEvent), message.UserId);
    }
}

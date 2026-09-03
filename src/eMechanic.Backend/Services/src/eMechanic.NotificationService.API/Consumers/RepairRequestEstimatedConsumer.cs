using eMechanic.Events.Events.RepairRequest;
using eMechanic.Events.Services;
using eMechanic.NotificationService.Constans;
using eMechanic.NotificationService.Helpers;
using eMechanic.NotificationService.Services.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

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
        var message = context.Message;

        _logger.LogInformation("{EventName} consumed. [UserId: {UserId}, RepairRequestId: {RepairRequestId}]",
            nameof(RepairRequestEstimatedEvent), message.UserId, message.RepairRequestId);

        var title = "Nowa Wycena Naprawy";
        var content = $@"
            <p>Mechanik przygotował kosztorys dla Twojego pojazdu.</p>
            <p>Szacowany koszt: <b>{message.EstimatedCost} {message.Currency}</b></p>
            <p>Zaloguj się do panelu eMechanic, aby zaakceptować lub odrzucić wycenę.</p>";

        var emailHtml = EmailTemplateBuilder.Build(title, content);

        await _dispatcher.DispatchAsync(
            message.UserId,
            EmailSubjects.RepairRequestEstimated,
            emailHtml,
            context.CancellationToken);

        _logger.LogInformation("Notification dispatched successfully for event {EventName}. [UserId: {UserId}]",
            nameof(RepairRequestEstimatedEvent), message.UserId);
    }
}

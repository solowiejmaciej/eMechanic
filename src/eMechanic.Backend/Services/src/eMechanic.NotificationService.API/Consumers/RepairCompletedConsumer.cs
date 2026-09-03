using eMechanic.Events.Events.Repair;
using eMechanic.Events.Services;
using eMechanic.NotificationService.Constans;
using eMechanic.NotificationService.Helpers;
using eMechanic.NotificationService.Services.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace eMechanic.NotificationService.Consumers;

public class RepairCompletedConsumer : IEventConsumer<RepairCompletedEvent>
{
    private readonly INotificationDispatcher _dispatcher;
    private readonly ILogger<RepairCompletedConsumer> _logger;

    public RepairCompletedConsumer(INotificationDispatcher dispatcher, ILogger<RepairCompletedConsumer> logger)
    {
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RepairCompletedEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation("{EventName} consumed. [UserId: {UserId}, RepairId: {RepairId}]",
            nameof(RepairCompletedEvent), message.UserId, message.RepairId);

        var title = "Naprawa zakończona";
        var content = $"<p>Naprawa o numerze <strong>{message.RepairId}</strong> została pomyślnie zakończona!</p>";

        var emailHtml = EmailTemplateBuilder.Build(title, content);

        await _dispatcher.DispatchAsync(
            message.UserId,
            EmailSubjects.RepairCompleted,
            emailHtml,
            context.CancellationToken);

        _logger.LogInformation("Notification dispatched successfully for event {EventName}. [UserId: {UserId}]",
            nameof(RepairCompletedEvent), message.UserId);
    }
}

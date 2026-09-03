using eMechanic.Events.Events.Repair;
using eMechanic.Events.Services;
using eMechanic.NotificationService.Constans;
using eMechanic.NotificationService.Helpers;
using eMechanic.NotificationService.Services.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace eMechanic.NotificationService.Consumers;

public class RepairStartedConsumer : IEventConsumer<RepairStartedEvent>
{
    private readonly INotificationDispatcher _dispatcher;
    private readonly ILogger<RepairStartedConsumer> _logger;

    public RepairStartedConsumer(INotificationDispatcher dispatcher, ILogger<RepairStartedConsumer> logger)
    {
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RepairStartedEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation("{EventName} consumed. [UserId: {UserId}, RepairId: {RepairId}]",
            nameof(RepairStartedEvent), message.UserId, message.RepairId);

        var title = "Prace nad pojazdem rozpoczęte!";
        var content = $@"
            <p>Twój pojazd wjechał właśnie na stanowisko warsztatowe i mechanik rozpoczął nad nim pracę.</p>
            <p><b>ID Naprawy:</b> {message.RepairId}</p>";

        var emailHtml = EmailTemplateBuilder.Build(title, content);

        await _dispatcher.DispatchAsync(
            message.UserId,
            EmailSubjects.RepairStarted,
            emailHtml,
            context.CancellationToken);

        _logger.LogInformation("Notification dispatched successfully for event {EventName}. [UserId: {UserId}]",
            nameof(RepairStartedEvent), message.UserId);
    }
}

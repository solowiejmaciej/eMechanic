using eMechanic.Events.Events.RepairRequest;
using eMechanic.Events.Services;
using eMechanic.NotificationService.Constans;
using eMechanic.NotificationService.Helpers;
using eMechanic.NotificationService.Services.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace eMechanic.NotificationService.Consumers;

public class WorkshopNewRepairConsumer : IEventConsumer<RepairRequestCreatedEvent>
{
    private readonly ILogger<WorkshopNewRepairConsumer> _logger;
    private readonly INotificationDispatcher _dispatcher;

    public WorkshopNewRepairConsumer(ILogger<WorkshopNewRepairConsumer> logger, INotificationDispatcher dispatcher)
    {
        _logger = logger;
        _dispatcher = dispatcher;
    }

    public async Task Consume(ConsumeContext<RepairRequestCreatedEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation("{EventName} consumed for workshop. [WorkshopId: {WorkshopId}, RepairRequestId: {RepairRequestId}]",
            nameof(RepairRequestCreatedEvent), message.WorkshopId, message.RepairRequestId);

        var title = "Nowe zlecenie naprawy!";
        var content = $@"
            <p>W systemie eMechanic pojawiło się nowe zgłoszenie dla Twojego warsztatu.</p>
            <p><b>ID Naprawy:</b> {message.RepairRequestId}</p>
            <p><b>ID Pojazdu:</b> {message.VehicleId}</p>
            <p>Zaloguj się do panelu warsztatu, aby przeanalizować zgłoszenie i przygotować wstępną diagnozę.</p>";

        var emailHtml = EmailTemplateBuilder.Build(title, content);

        await _dispatcher.DispatchToWorkshopAsync(
            message.WorkshopId,
            EmailSubjects.WorkshopNewRepair,
            emailHtml,
            context.CancellationToken);

        _logger.LogInformation("Notification dispatched successfully for event {EventName}. [WorkshopId: {WorkshopId}]",
            nameof(RepairRequestCreatedEvent), message.WorkshopId);
    }
}

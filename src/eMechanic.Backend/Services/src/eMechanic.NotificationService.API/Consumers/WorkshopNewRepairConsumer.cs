using MassTransit;
using eMechanic.Events.Events.RepairRequest;
using eMechanic.NotificationService.Services.Abstractions;

namespace eMechanic.NotificationService.Consumers;

public class WorkshopNewRepairConsumer : IConsumer<RepairRequestCreatedEvent>
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
        var msg = context.Message;
        _logger.LogInformation("WorkshopNewRepairConsumer consumed for request {RequestId}", msg.RepairRequestId);

        var html = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #eee; padding: 20px; border-top: 5px solid #2980b9;'>
                <h2 style='color: #2c3e50;'>Nowe zlecenie naprawy!</h2>
                <p>W systemie eMechanic pojawiło się nowe zgłoszenie dla Twojego warsztatu.</p>
                <hr style='border: 0; border-top: 1px solid #eee;' />
                <p><b>ID Naprawy:</b> {msg.RepairRequestId}</p>
                <p><b>ID Pojazdu:</b> {msg.VehicleId}</p>
                <p>Zaloguj się do panelu warsztatu, aby przeanalizować zgłoszenie i przygotować wstępną diagnozę.</p>
                <a href='#' style='display: inline-block; background: #2980b9; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; margin-top: 10px;'>Przejdź do zleceń</a>
            </div>";

        await _dispatcher.DispatchToWorkshopAsync(msg.WorkshopId, "Nowe zlecenie naprawy", html, context.CancellationToken);
    }



}

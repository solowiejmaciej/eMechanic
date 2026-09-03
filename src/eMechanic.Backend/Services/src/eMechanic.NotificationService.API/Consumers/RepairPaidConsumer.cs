using eMechanic.Events.Events.Repair;
using eMechanic.Events.Services;
using eMechanic.NotificationService.Constans;
using eMechanic.NotificationService.Helpers;
using eMechanic.NotificationService.Services.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace eMechanic.NotificationService.Consumers;

public class RepairPaidConsumer : IEventConsumer<RepairPaidEvent>
{
    private readonly INotificationDispatcher _dispatcher;
    private readonly ILogger<RepairPaidConsumer> _logger;

    public RepairPaidConsumer(INotificationDispatcher dispatcher, ILogger<RepairPaidConsumer> logger)
    {
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RepairPaidEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation("{EventName} consumed. [UserId: {UserId}, RepairId: {RepairId}]",
            nameof(RepairPaidEvent), message.UserId, message.RepairId);

        var title = "Płatność zaksięgowana!";
        var content = $@"
            <p>Cześć {message.UserFirstName},</p>
            <p>Otrzymaliśmy Twoją płatność za naprawę pojazdu <b>{message.VehicleManufacturer} {message.VehicleModel}</b> ({message.VehicleLicensePlate}).</p>
            <p>Kwota: <b>{message.FinalCostAmount} {message.FinalCostCurrency}</b></p>
            <p>Możesz już odebrać swój pojazd. Dziękujemy za korzystanie z systemu eMechanic!</p>";

        var emailHtml = EmailTemplateBuilder.Build(title, content);

        await _dispatcher.DispatchAsync(
            message.UserId,
            EmailSubjects.RepairPaid,
            emailHtml,
            context.CancellationToken);

        _logger.LogInformation("Notification dispatched successfully for event {EventName}. [UserId: {UserId}]",
            nameof(RepairPaidEvent), message.UserId);
    }
}

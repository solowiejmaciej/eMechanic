using eMechanic.Events.Events.Workshop;
using eMechanic.Events.Services;
using eMechanic.NotificationService.DAL;
using eMechanic.NotificationService.DAL.Entities;
using eMechanic.NotificationService.Constans;
using eMechanic.NotificationService.Helpers;
using eMechanic.NotificationService.Services.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace eMechanic.NotificationService.Consumers;

public class WorkshopCreatedConsumer : IEventConsumer<WorkshopCreatedEvent>
{
    private readonly ILogger<WorkshopCreatedConsumer> _logger;
    private readonly NotificationDbContext _dbContext;
    private readonly INotificationDispatcher _dispatcher;

    public WorkshopCreatedConsumer(ILogger<WorkshopCreatedConsumer> logger, NotificationDbContext dbContext, INotificationDispatcher dispatcher)
    {
        _logger = logger;
        _dbContext = dbContext;
        _dispatcher = dispatcher;
    }

    public async Task Consume(ConsumeContext<WorkshopCreatedEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation("{EventName} consumed. [WorkshopId: {WorkshopId}]", nameof(WorkshopCreatedEvent), message.WorkshopId);

        if (await _dbContext.Workshops.AnyAsync(w => w.Id == message.WorkshopId))
        {
            _logger.LogWarning("Workshop with ID: {WorkshopId} already exists in notification database.", message.WorkshopId);
            return;
        }

        var workshop = new NotificationWorkshop
        {
            Id = message.WorkshopId,
            Name = message.Name,
            Email = message.Email,
            PhoneNumber = null,
            EmailEnabled = true,
            SmsEnabled = false
        };

        _dbContext.Workshops.Add(workshop);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Workshop {WorkshopId} has been registered in notification system.", message.WorkshopId);

        var title = "Witaj w eMechanic!";
        var content = $@"
            <p>Dziękujemy za dołączenie do sieci warsztatów w systemie <strong>eMechanic</strong>.</p>
            <p>Od teraz będziesz otrzymywać powiadomienia o nowych zgłoszeniach bezpośrednio tutaj.</p>
            <br/>
            <p>Pozdrawiamy,<br/>Zespół eMechanic</p>";

        var emailHtml = EmailTemplateBuilder.Build(title, content);

        await _dispatcher.DispatchToWorkshopAsync(
            message.WorkshopId,
            EmailSubjects.WelcomeWorkshop,
            emailHtml,
            context.CancellationToken);

        _logger.LogInformation("Notification dispatched successfully for event {EventName}. [WorkshopId: {WorkshopId}]",
            nameof(WorkshopCreatedEvent), message.WorkshopId);
    }
}

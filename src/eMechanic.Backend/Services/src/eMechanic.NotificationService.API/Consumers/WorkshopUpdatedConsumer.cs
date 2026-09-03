using eMechanic.Events.Events.Workshop;
using eMechanic.Events.Services;
using eMechanic.NotificationService.DAL;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace eMechanic.NotificationService.Consumers;

public class WorkshopUpdatedConsumer : IEventConsumer<WorkshopUpdatedEvent>
{
    private readonly NotificationDbContext _dbContext;
    private readonly ILogger<WorkshopUpdatedConsumer> _logger;

    public WorkshopUpdatedConsumer(NotificationDbContext dbContext, ILogger<WorkshopUpdatedConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<WorkshopUpdatedEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation("{EventName} consumed. [WorkshopId: {WorkshopId}]", nameof(WorkshopUpdatedEvent), message.WorkshopId);

        var workshop = await _dbContext.Workshops.FirstOrDefaultAsync(w => w.Id == message.WorkshopId);

        if (workshop == null)
        {
            _logger.LogWarning("Event {EventName} received for a non-existent workshop: {WorkshopId}",
                nameof(WorkshopUpdatedEvent), message.WorkshopId);
            return;
        }

        workshop.Email = message.Email;
        workshop.PhoneNumber = message.PhoneNumber;

        _dbContext.Workshops.Update(workshop);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Contact information for workshop: {WorkshopId} has been updated in the notification module.", message.WorkshopId);
    }
}

using eMechanic.Events.Events.Workshop;
using eMechanic.NotificationService.DAL;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace eMechanic.NotificationService.Consumers;

public class WorkshopUpdatedConsumer : IConsumer<WorkshopUpdatedEvent>
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
        var msg = context.Message;
        _logger.LogInformation("WorkshopUpdatedEvent was consumed {WorkshopId}", msg.WorkshopId);

        var workshop = await _dbContext.Workshops.FirstOrDefaultAsync(w => w.Id == msg.WorkshopId);

        if (workshop == null)
        {
            _logger.LogWarning("WorkshopUpdatedEvent was received for a non-existent workshop: {WorkshopId}", msg.WorkshopId);
            return;
        }

        workshop.Email = msg.Email;
        workshop.PhoneNumber = msg.PhoneNumber;

        _dbContext.Workshops.Update(workshop);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Contact inormation for the workshop {WorkshopId} has been updated in the notification module.", msg.WorkshopId);    }
}


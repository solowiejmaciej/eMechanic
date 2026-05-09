using eMechanic.Events.Events.Workshop;
using eMechanic.NotificationService.DAL;
using eMechanic.NotificationService.DAL.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace eMechanic.NotificationService.Consumers;

public class WorkshopCreatedConsumer : IConsumer<WorkshopCreatedEvent>
{
    private readonly ILogger<WorkshopCreatedConsumer> _logger;
    private readonly NotificationDbContext _dbContext;

    public WorkshopCreatedConsumer(ILogger<WorkshopCreatedConsumer> logger, NotificationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<WorkshopCreatedEvent> context)
    {
        var msg = context.Message;

        if (await _dbContext.Users.AnyAsync(w => w.Id == msg.WorkshopId))
        {
            _logger.LogWarning("Warsztat o ID {WorkshopId} już istnieje w bazie powiadomień.", msg.WorkshopId);
            return;
        }

        var workshop = new NotificationWorkshop
            {
                Id = msg.WorkshopId,
                Name = msg.Name,
                Email = msg.Email,
                PhoneNumber = null,
                EmailEnabled = true,
                SmsEnabled = false
            };
            _dbContext.Workshops.Add(workshop);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Zarejestrowano warsztat w systenie powiadomień: {Name}", msg.Name);
    }
}

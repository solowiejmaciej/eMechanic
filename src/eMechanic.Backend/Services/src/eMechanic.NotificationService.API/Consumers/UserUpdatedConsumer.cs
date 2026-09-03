using eMechanic.Events.Events.User;
using eMechanic.Events.Services;
using eMechanic.NotificationService.DAL;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace eMechanic.NotificationService.Consumers;

public class UserUpdatedConsumer : IEventConsumer<UserUpdatedEvent>
{
    private readonly NotificationDbContext _dbContext;
    private readonly ILogger<UserUpdatedConsumer> _logger;

    public UserUpdatedConsumer(NotificationDbContext dbContext, ILogger<UserUpdatedConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserUpdatedEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation("{EventName} consumed. [UserId: {UserId}]", nameof(UserUpdatedEvent), message.UserId);

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == message.UserId);

        if (user == null)
        {
            _logger.LogWarning("Event {EventName} received for a non-existent user: {UserId}",
                nameof(UserUpdatedEvent), message.UserId);
            return;
        }

        user.Email = message.Email;

        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Contact information for user: {UserId} has been updated in the notification module.", message.UserId);
    }
}

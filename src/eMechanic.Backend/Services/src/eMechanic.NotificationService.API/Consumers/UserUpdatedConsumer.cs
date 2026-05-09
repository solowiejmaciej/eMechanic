using eMechanic.NotificationService.DAL;
using eMechanic.Events.Events.User;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace eMechanic.NotificationService.Consumers;

public class UserUpdatedConsumer : IConsumer<UserUpdatedEvent>
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
        var msg = context.Message;

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == msg.UserId);

        if (user == null)
        {
            _logger.LogWarning("UserUpdatedEvent was receiver for a non-exist user: {UserId}", msg.UserId);
            return;
        }

        user.Email = msg.Email;

        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Contact information for user: {UserId} has been updated in the notification module.", msg.UserId);
    }

}

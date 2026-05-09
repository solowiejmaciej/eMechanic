using MassTransit;
using eMechanic.Events.Events.User;
using eMechanic.NotificationService.DAL.Entities;
using eMechanic.NotificationService.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace eMechanic.NotificationService.Consumers;


public class UserCreatedConsumer : IConsumer<UserCreatedEvent>
{
    private readonly NotificationDbContext _dbContext;
    private readonly ILogger<UserCreatedConsumer> _logger;

    public UserCreatedConsumer(NotificationDbContext dbContext, ILogger<UserCreatedConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var msg = context.Message;

        //sprawdzmy czy juz go nie ma
        if (await _dbContext.Users.AnyAsync(u => u.Id == msg.UserId)) return;

        var user = new NotificationUser
        {
            Id = msg.UserId, Email = msg.Email, EmailEnabled = true, SmsEnabled = true,
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Serwis powiadomień zarejestrował usera: {UserId}", msg.UserId);
    }

}

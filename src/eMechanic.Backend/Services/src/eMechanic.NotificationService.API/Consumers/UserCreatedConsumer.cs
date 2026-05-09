using MassTransit;
using eMechanic.Events.Events.User;
using eMechanic.NotificationService.DAL.Entities;
using eMechanic.NotificationService.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace eMechanic.NotificationService.Consumers;

using Services.Abstractions;

public class UserCreatedConsumer : IConsumer<UserCreatedEvent>
{
    private readonly NotificationDbContext _dbContext;
    private readonly INotificationDispatcher _dispatcher;
    private readonly ILogger<UserCreatedConsumer> _logger;

    public UserCreatedConsumer(NotificationDbContext dbContext, ILogger<UserCreatedConsumer> logger, INotificationDispatcher dispatcher)
    {
        _dbContext = dbContext;
        _logger = logger;
        _dispatcher = dispatcher;
    }

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("UserCreatedEvent was consumed {EventUserId}", msg.UserId);

        var userExists = await _dbContext.Users.AnyAsync(u => u.Id == msg.UserId);

        if (!userExists)
        {
            var user = new NotificationUser
            {
                Id = msg.UserId,
                Email = msg.Email,
                EmailEnabled = true,
                SmsEnabled = false,
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("User {UserId} has been added to the notification database.", msg.UserId);
        }

        var welcomeSubject = "Witaj w eMechanic!";
        var welcomeMessage = $@"
            <div style='font-family: sans-serif; padding: 20px; border: 1px solid #ddd;'>
                <h2 style='color: #2c3e50;'>Cześć!</h2>
                <p>Dziękujemy za założenie konta w systemie <strong>eMechanic</strong>.</p>
                <p>Od teraz będziesz otrzymywać powiadomienia o statusie swoich napraw bezpośrednio tutaj.</p>
                <br/>
                <p>Pozdrawiamy,<br/>Zespół eMechanic</p>
            </div>";

        await _dispatcher.DispatchAsync(
            msg.UserId,
            welcomeSubject,
            welcomeMessage,
            context.CancellationToken);

        _logger.LogInformation("Welcome message has been sent to the user: {UserId}", msg.UserId);

    }

}

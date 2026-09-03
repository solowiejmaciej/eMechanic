using eMechanic.Events.Events.User;
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

public class UserCreatedConsumer : IEventConsumer<UserCreatedEvent>
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
        var message = context.Message;

        _logger.LogInformation("{EventName} consumed. [UserId: {UserId}]", nameof(UserCreatedEvent), message.UserId);

        var userExists = await _dbContext.Users.AnyAsync(u => u.Id == message.UserId);

        if (!userExists)
        {
            var user = new NotificationUser
            {
                Id = message.UserId,
                Email = message.Email,
                EmailEnabled = true,
                SmsEnabled = false,
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("User {UserId} has been added to the notification database.", message.UserId);
        }

        var title = "Cześć!";
        var content = $@"
            <p>Dziękujemy za założenie konta w systemie <strong>eMechanic</strong>.</p>
            <p>Od teraz będziesz otrzymywać powiadomienia o statusie swoich napraw bezpośrednio tutaj.</p>
            <br/>
            <p>Pozdrawiamy,<br/>Zespół eMechanic</p>";

        var emailHtml = EmailTemplateBuilder.Build(title, content);

        await _dispatcher.DispatchAsync(
            message.UserId,
            EmailSubjects.WelcomeUser,
            emailHtml,
            context.CancellationToken);

        _logger.LogInformation("Notification dispatched successfully for event {EventName}. [UserId: {UserId}]",
            nameof(UserCreatedEvent), message.UserId);
    }
}

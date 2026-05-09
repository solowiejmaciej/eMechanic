using eMechanic.Events.Events.User;
using eMechanic.Events.Services;
using eMechanic.NotificationService.Services.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace eMechanic.NotificationService.Consumers;

using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

public class NotificationUserCreatedConsumer : IEventConsumer<UserCreatedEvent>
{
    private readonly NotificationDbContext _dbContext;
    private readonly INotificationDispatcher _dispatcher;
    private readonly ILogger<NotificationUserCreatedConsumer> _logger;

    public NotificationUserCreatedConsumer(NotificationDbContext dbContext,
        INotificationDispatcher dispatcher,
        ILogger<NotificationUserCreatedConsumer> logger)
    {
        _dbContext = dbContext;
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var msg = context.Message;

        if (!await _dbContext.Users.AnyAsync(u => u.Id == msg.UserId))
        {
            var user = new NotificationUser
            {
                Id = msg.UserId,
                Email = msg.Email,
                EmailEnabled = true,
                SmsEnabled = true,
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }
        _logger.LogInformation("Zapisano dane kontaktowe dla nowego użytkownika: {UserId}", msg.UserId);

        var welcomeMessage = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #eee; padding: 20px;'>
                <h2 style='color: #e67e22; text-align: center;'>Witaj w eMechanic!</h2>
                <p>Cieszymy się, że do nas dołączyłeś.</p>
                <p>Twój profil został utworzony pomyślnie dla adresu: <b>{msg.Email}</b>.</p>
                <p>Od teraz możesz:</p>
                <ul>
                    <li>Dodawać swoje pojazdy do wirtualnego garażu.</li>
                    <li>Szybko zgłaszać usterki do sprawdzonych warsztatów.</li>
                    <li>Akceptować wyceny i kontrolować koszty naprawy online.</li>
                </ul>
            </div>";

        await _dispatcher.DispatchAsync(
            msg.UserId,
            "Witaj w eMechanic!",
            welcomeMessage,
            context.CancellationToken);

        _logger.LogInformation("Wysłano maila powitalnego do użytkownika: {UserId}", msg.UserId);
    }
}

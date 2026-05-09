using eMechanic.NotificationService.DAL;
using Microsoft.EntityFrameworkCore;
using eMechanic.NotificationService.Services.Abstractions;

namespace eMechanic.NotificationService.Services;


public class NotificationDispatcher : INotificationDispatcher
{
    private readonly NotificationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly ILogger<NotificationDispatcher> _logger;

    public NotificationDispatcher(
        NotificationDbContext context,
        IEmailService emailService,
        ISmsService smsService,
        ILogger<NotificationDispatcher> logger)
    {
        _context = context;
        _emailService = emailService;
        _smsService = smsService;
        _logger = logger;
    }

    public async Task DispatchAsync(Guid userId, string subject, string message, CancellationToken ct = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user == null)
        {
            _logger.LogWarning("Nie znaleziono użytkownika {UserId} w bazie powiadomień.", userId);
            return;
        }

        if (user.EmailEnabled && !string.IsNullOrEmpty(user.Email))
        {
            await _emailService.SendEmailAsync(user.Email, subject, message, cancellationToken: ct);
        }

       if (user.SmsEnabled && !string.IsNullOrEmpty(user.PhoneNumber))
       {
           await _smsService.SendSmsAsync(user.PhoneNumber, message, ct);
       }
    }

    public async Task DispatchToWorkshopAsync(Guid workshopId, string subject, string message, CancellationToken ct = default)
    {
        var workshop = await _context.Workshops.FirstOrDefaultAsync(w => w.Id == workshopId, ct);
        if (workshop == null)
        {
            _logger.LogWarning("Nie znaleziono warsztatu {WorkshopId} w bazie powiadomień.", workshopId);
            return;
        }

        if (workshop.EmailEnabled && !string.IsNullOrEmpty(workshop.Email))
        {
            await _emailService.SendEmailAsync(workshop.Email, subject, message, cancellationToken: ct);
        }

        if (workshop.SmsEnabled && !string.IsNullOrEmpty(workshop.PhoneNumber))
        {
            await _smsService.SendSmsAsync(workshop.PhoneNumber, message, ct);
        }
    }
}

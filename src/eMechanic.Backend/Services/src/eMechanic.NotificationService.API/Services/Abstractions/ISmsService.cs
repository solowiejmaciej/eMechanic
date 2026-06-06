namespace eMechanic.NotificationService.Services.Abstractions;

public interface ISmsService
{
    Task SendSmsAsync(
        string phoneNumber,
        string message,
        CancellationToken cancellationToken = default);
}

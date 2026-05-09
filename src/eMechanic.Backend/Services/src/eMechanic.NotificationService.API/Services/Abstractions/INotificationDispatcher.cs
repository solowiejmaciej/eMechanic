namespace eMechanic.NotificationService.Services.Abstractions;

public interface INotificationDispatcher
{
    Task DispatchAsync(
        Guid userId,
        string subject,
        string message,
        CancellationToken ct = default
    );

    Task DispatchToWorkshopAsync(
        Guid workshopId,
        string subject,
        string message,
        CancellationToken ct = default
    );

}

namespace eMechanic.NotificationService.DAL.Entities;

public class NotificationWorkshop
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }

    public bool EmailEnabled { get; set; } = true;
    public bool SmsEnabled { get; set; } = true;
}

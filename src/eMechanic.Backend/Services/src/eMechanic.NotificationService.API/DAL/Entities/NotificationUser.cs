namespace eMechanic.NotificationService.DAL.Entities;

public class NotificationUser
{
    public Guid Id { get; set; }
    public string? Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool EmailEnabled { get; set; } = true;
    public bool SmsEnabled { get; set; } = true;
}

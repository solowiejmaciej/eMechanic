namespace eMechanic.NotificationService.Services;

/// <summary>
/// Klasa konfiguracyjna przechowująca klucze API i ustawienia nadawców
/// dla systemów e-mail oraz SMS.
/// </summary>


public class NotificationSettings
{
    public const string SECTION_NAME = "Notifications";

    ///EmailLabs

    public string EmailLabsAppId { get; set; } = string.Empty;
    public string EmailLabsSecretKey { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = "no-reply@emechanic.pl";
    public string SenderName { get; set; } = "eMechanic";

    //Twilio SMS

    public string SmsAccountSid { get; set; } = string.Empty;
    public string SmsAuthToken { get; set; } = string.Empty;
    public string SmsSenderNumber { get; set; } = string.Empty;
}

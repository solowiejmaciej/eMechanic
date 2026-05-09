using eMechanic.NotificationService.Services.Abstractions;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;


namespace eMechanic.NotificationService.Services.Infrastructure;

public class TwilioSmsService : ISmsService
{
    private readonly NotificationSettings _settings;
    private readonly ILogger<TwilioSmsService> _logger;

    public TwilioSmsService(IOptions<NotificationSettings> settings, ILogger<TwilioSmsService> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        TwilioClient.Init(_settings.SmsAccountSid, _settings.SmsAuthToken);
    }

    public async Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        try
        {
            var messageResource = await MessageResource.CreateAsync(
                body: message,
                from: new PhoneNumber(_settings.SmsSenderNumber),
                to: new PhoneNumber(phoneNumber)
            );

            _logger.LogInformation("SMS sent to {Phone}. SID: {Sid}", phoneNumber, messageResource.Sid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SMS to {Phone}", phoneNumber);
            throw;
        }
    }
}

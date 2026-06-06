using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using eMechanic.NotificationService.Services.Abstractions;
using eMechanic.NotificationService.Services;

namespace eMechanic.NotificationService.Services.Infrastructure;

public class EmailLabsApiService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly NotificationSettings _settings;
    private readonly ILogger<EmailLabsApiService> _logger;

    // Ilogger
    public EmailLabsApiService(
        HttpClient httpClient,
        IOptions<NotificationSettings> settings,
        ILogger<EmailLabsApiService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true,
        CancellationToken cancellationToken = default)
    {
        var authString = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{_settings.EmailLabsAppId}:{_settings.EmailLabsSecretKey}"));

        var values = new Dictionary<string, string>
        {
            { "to", to },
            { "subject", subject },
            { "html", isHtml ? body : "" },
            { "text", !isHtml ? body : "" },
            { "from", _settings.SenderEmail },
            { "from_name", _settings.SenderName }
        };

        var content = new FormUrlEncodedContent(values);

        using var request =
            new HttpRequestMessage(HttpMethod.Post, "https://api.emaillabs.io/v1/send_mail") { Content = content };
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authString);

        try
        {
            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("EmailLabs API error: {Status} - {Detail}", response.StatusCode, error);
                throw new InvalidOperationException("The email could not be sent.");
            }

            _logger.LogInformation("E-mail to {To} was sent successfully.", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while communicating with EmailLabs for the address {To}", to);
            throw;
        }
    }
}

namespace eMechanic.Infrastructure.Services.Identity.ExternalProviders.Google;

using Microsoft.Extensions.Configuration;
using GoogleJsonWebSignature = global::Google.Apis.Auth.GoogleJsonWebSignature;
internal interface IGoogleTokenValidator
{
    Task<GoogleJsonWebSignature.Payload?> ValidateAndGetPayload(string idToken);
}

internal class GoogleTokenValidator : IGoogleTokenValidator
{
    private readonly string _clientId;

    public GoogleTokenValidator(IConfiguration configuration)
    {
        _clientId = configuration["Authentication:Google:ClientId"]
                    ?? throw new ArgumentNullException("Authentication:Google:ClientId is missing");
    }

    public async Task<GoogleJsonWebSignature.Payload?> ValidateAndGetPayload(string idToken)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings { Audience = [_clientId] };
        return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
    }
}

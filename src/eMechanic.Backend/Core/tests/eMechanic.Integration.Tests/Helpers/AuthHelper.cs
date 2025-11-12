namespace eMechanic.Integration.Tests.Helpers;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Token.Features.Create.User;
using Application.Token.Features.Create.Workshop;
using Application.Users.Features.Create;
using Application.Workshop.Features.Create;
using eMechanic.API.Constans;
using eMechanic.API.Features.Tokens;
using eMechanic.API.Features.User;
using eMechanic.API.Features.Workshop;
using FluentAssertions;

public class AuthHelper
{
    private readonly HttpClient _client;
    private const string _baseApiUrl = $"/api/{WebApiConstans.CURRENT_API_VERSION}";

    public AuthHelper(HttpClient client)
    {
        _client = client;
    }

    public async Task<FullAuthResponse> CreateAndLoginUserAsync(string? email = null, string? password = null)
    {
        var userEmail = email ?? $"test-user-{Guid.NewGuid()}@integration.com";
        var userPassword = password ?? "Password123!";

        var registerCmd = new CreateUserCommand("Test", "User", userEmail, userPassword);
        var registerUrl = $"{_baseApiUrl}{UserPrefix.CREATE_USER_ENDPOINT}";
        var registerResponse = await _client.PostAsJsonAsync(registerUrl, registerCmd);
        registerResponse.EnsureSuccessStatusCode();

        var registerContent = await registerResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var userId = registerContent!["userId"];


        var loginCmd = new CreateUserTokenCommand(userEmail, userPassword);
        var loginUrl = $"{_baseApiUrl}{TokenPrefix.CREATE_USER_TOKEN_ENDPOINT}";
        var loginResponse = await _client.PostAsJsonAsync(loginUrl, loginCmd);
        loginResponse.EnsureSuccessStatusCode();

        var loginContent = await loginResponse.Content.ReadFromJsonAsync<CreateUserTokenResponse>();
        loginContent.Should().NotBeNull();
        loginContent!.Token.Should().NotBeNullOrWhiteSpace();

        return new FullAuthResponse(loginContent.UserId, loginContent.Token, loginContent.RefreshToken);
    }

    public async Task<FullAuthResponse> CreateAndLoginWorkshopAsync(string? email = null, string? password = null)
    {
        var workshopEmail = email ?? $"test-workshop-{Guid.NewGuid()}@integration.com";
        var workshopPassword = password ?? "Password123!";
        var uniqueSuffix = Guid.NewGuid().ToString("N")[..6];

        var registerCmd = new CreateWorkshopCommand(
            workshopEmail, workshopPassword, $"contact-{uniqueSuffix}@workshop.com",
            $"Test Workshop {uniqueSuffix}", $"TestW-{uniqueSuffix}", "123456789",
            "Test St 1", "Test City", "12-345", "Testland");
        var registerUrl = $"{_baseApiUrl}{WorkshopPrefix.CREATE_ENDPOINT}";
        var registerResponse = await _client.PostAsJsonAsync(registerUrl, registerCmd);
        registerResponse.EnsureSuccessStatusCode();

        var registerContent = await registerResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var workshopId = registerContent!["workshopId"];


        var loginCmd = new CreateWorkshopTokenCommand(workshopEmail, workshopPassword);
        var loginUrl = $"{_baseApiUrl}{TokenPrefix.CREATE_WORKSHOP_TOKEN_ENDPOINT}";
        var loginResponse = await _client.PostAsJsonAsync(loginUrl, loginCmd);
        loginResponse.EnsureSuccessStatusCode();

        var loginContent = await loginResponse.Content.ReadFromJsonAsync<CreateWorkshopTokenResponse>();
        loginContent.Should().NotBeNull();
        loginContent!.Token.Should().NotBeNullOrWhiteSpace();

        return new FullAuthResponse(loginContent.WorkshopId, loginContent.Token, loginContent.RefreshToken);
    }

    public void SetAuthHeader(string token) => _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    public void ClearAuthHeader() => _client.DefaultRequestHeaders.Authorization = null;
}

public static class HttpClientExtensions
{
    public static void SetBearerToken(this HttpClient client, string token) => client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    public static void ClearBearerToken(this HttpClient client) => client.DefaultRequestHeaders.Authorization = null;
}

namespace eMechanic.Integration.Tests.Helpers;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using eMechanic.Application.Users.Login;
using eMechanic.Application.Users.Register;
using eMechanic.Application.Workshop.Login;
using eMechanic.Application.Workshop.Register;
using FluentAssertions;

public class AuthHelper
{
    private readonly HttpClient _client;

    public AuthHelper(HttpClient client)
    {
        _client = client;
    }

    public async Task<(Guid UserId, string Token)> CreateAndLoginUserAsync(string? email = null, string? password = null)
    {
        var userEmail = email ?? $"test-user-{Guid.NewGuid()}@integration.com";
        var userPassword = password ?? "Password123!";

        var registerCmd = new RegisterUserCommand("Test", "User", userEmail, userPassword);
        var registerResponse = await _client.PostAsJsonAsync("/api/v1/users/register", registerCmd);
        registerResponse.EnsureSuccessStatusCode();

        var registerContent = await registerResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var userId = registerContent!["userId"];


        var loginCmd = new LoginUserCommand(userEmail, userPassword);
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/users/login", loginCmd);
        loginResponse.EnsureSuccessStatusCode();

        var loginContent = await loginResponse.Content.ReadFromJsonAsync<LoginUserResponse>();
        loginContent.Should().NotBeNull();
        loginContent!.Token.Should().NotBeNullOrWhiteSpace();

        return (userId, loginContent.Token);
    }

     public async Task<(Guid WorkshopId, string Token)> CreateAndLoginWorkshopAsync(string? email = null, string? password = null)
    {
        var workshopEmail = email ?? $"test-workshop-{Guid.NewGuid()}@integration.com";
        var workshopPassword = password ?? "Password123!";
        var uniqueSuffix = Guid.NewGuid().ToString("N")[..6];

        var registerCmd = new RegisterWorkshopCommand(
            workshopEmail, workshopPassword, $"contact-{uniqueSuffix}@workshop.com",
            $"Test Workshop {uniqueSuffix}", $"TestW-{uniqueSuffix}", "123456789",
            "Test St 1", "Test City", "12-345", "Testland");
        var registerResponse = await _client.PostAsJsonAsync("/api/v1/workshops/register", registerCmd);
        registerResponse.EnsureSuccessStatusCode();

         var registerContent = await registerResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var workshopId = registerContent!["workshopId"];


        var loginCmd = new LoginWorkshopCommand(workshopEmail, workshopPassword);
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/workshops/login", loginCmd);
        loginResponse.EnsureSuccessStatusCode();

        var loginContent = await loginResponse.Content.ReadFromJsonAsync<LoginWorkshopResponse>();
        loginContent.Should().NotBeNull();
        loginContent!.Token.Should().NotBeNullOrWhiteSpace();

        return (workshopId, loginContent.Token);
    }

    public void SetAuthHeader(string token) => _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    public void ClearAuthHeader() => _client.DefaultRequestHeaders.Authorization = null;
}

public static class HttpClientExtensions
{
     public static void SetBearerToken(this HttpClient client, string token) => client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

     public static void ClearBearerToken(this HttpClient client) => client.DefaultRequestHeaders.Authorization = null;
}

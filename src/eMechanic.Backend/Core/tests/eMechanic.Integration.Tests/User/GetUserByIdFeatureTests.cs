namespace eMechanic.Integration.Tests.User;

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TestContainers;

public class GetUserByIdFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;

    public GetUserByIdFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetUserById_Should_ReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();
        // Act
        var response = await _client.GetAsync($"/api/v1/users/{nonExistentUserId}");
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUserById_Should_ReturnOk_WhenUserExists()
    {
        // Arrange
        var command = new Application.Users.Register.RegisterUserCommand(
            "Jan",
            "Tester",
            $"test-{Guid.NewGuid()}@integration.com",
            "zaq1@WSX");

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/users/register", command);

        var registeredUser = await registerResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var userId = registeredUser!["userId"];
        // Act
        var response = await _client.GetAsync($"/api/v1/users/{userId}");
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

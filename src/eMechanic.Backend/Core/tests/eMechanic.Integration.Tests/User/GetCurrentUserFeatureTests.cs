namespace eMechanic.Integration.Tests.User;

using System.Net;
using System.Net.Http.Json;
using Application.Users.Features.Get.Current;
using FluentAssertions;
using Helpers;
using TestContainers;

public class GetCurrentUserFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly AuthHelper _authHelper;

    public GetCurrentUserFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _authHelper = new AuthHelper(_client);
    }

    [Fact]
    public async Task GetCurrentUser_Should_ReturnOkAndUserData_WhenUserIsAuthenticated()
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginUserAsync($"current-user-{Guid.NewGuid()}@int.com");
        _client.SetBearerToken(authResponse.Token);

        // Act
        var response = await _client.GetAsync("/api/v1/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var userResponse = await response.Content.ReadFromJsonAsync<GetCurrentUserResponse>();
        userResponse.Should().NotBeNull();
        userResponse!.Id.Should().Be(authResponse.DomainId);
        userResponse.Email.Should().Contain("@int.com");
    }

    [Fact]
    public async Task GetCurrentUser_Should_ReturnUnauthorized_WhenTokenIsMissing()
    {
        // Arrange
        _client.ClearBearerToken();

        // Act
        var response = await _client.GetAsync("/api/v1/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCurrentUser_Should_ReturnForbidden_WhenWorkshopTokenIsUsed()
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginWorkshopAsync($"workshop-auth-{Guid.NewGuid()}@int.com");
        _client.SetBearerToken(authResponse.Token);

        // Act
        var response = await _client.GetAsync("/api/v1/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}

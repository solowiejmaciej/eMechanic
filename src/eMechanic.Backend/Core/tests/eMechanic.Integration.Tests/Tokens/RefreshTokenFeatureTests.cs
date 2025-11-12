namespace eMechanic.Integration.Tests.Tokens;

using System.Net;
using System.Net.Http.Json;
using API.Constans;
using API.Features.Tokens;
using eMechanic.API.Features.Tokens.Refresh.Request;
using eMechanic.Application.Token.Features.Refresh;
using eMechanic.Integration.Tests.Helpers;
using eMechanic.Integration.Tests.TestContainers;
using FluentAssertions;

public class RefreshTokenFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly AuthHelper _authHelper;
    private const string REFRESH_URL = "/api/" + WebApiConstans.CURRENT_API_VERSION + TokenPrefix.REFRESH_TOKEN_ENDPOINT;

    public RefreshTokenFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _authHelper = new AuthHelper(_client);
    }

    [Fact]
    public async Task RefreshToken_Should_ReturnNewTokens_WhenTokensAreValid()
    {
        // Arrange
        var authResponse1 = await _authHelper.CreateAndLoginUserAsync();
        var request = new RefreshTokenRequest(authResponse1.RefreshToken, authResponse1.Token);

        // Act
        var response = await _client.PostAsJsonAsync(REFRESH_URL, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var authResponse2 = await response.Content.ReadFromJsonAsync<RefreshTokenResponse>();

        authResponse2.Should().NotBeNull();
        authResponse2.Token.Should().NotBeNullOrWhiteSpace();
        authResponse2.RefreshToken.Should().NotBeNullOrWhiteSpace();

        authResponse2.Token.Should().NotBe(authResponse1.Token);
        authResponse2.RefreshToken.Should().NotBe(authResponse1.RefreshToken);
    }

    [Fact]
    public async Task RefreshToken_Should_ReturnUnauthorized_WhenOldRefreshTokenIsReused()
    {
        // Arrange
        var authResponse1 = await _authHelper.CreateAndLoginUserAsync();
        var request1 = new RefreshTokenRequest(authResponse1.RefreshToken, authResponse1.Token);

        var response2 = await _client.PostAsJsonAsync(REFRESH_URL, request1);
        response2.EnsureSuccessStatusCode();

        // Act
        var response3 = await _client.PostAsJsonAsync(REFRESH_URL, request1);

        // Assert
        response3.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefreshToken_Should_ReturnBadRequest_WhenRefreshTokenIsInvalid()
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginUserAsync();
        var request = new RefreshTokenRequest("to-jest-zly-token", authResponse.Token);

        // Act
        var response = await _client.PostAsJsonAsync(REFRESH_URL, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefreshToken_Should_ReturnNewTokens_WhenWorkshopTokensAreValid()
    {
        // Arrange
        var authResponse1 = await _authHelper.CreateAndLoginWorkshopAsync();
        var request = new RefreshTokenRequest(authResponse1.RefreshToken, authResponse1.Token);

        // Act
        var response = await _client.PostAsJsonAsync(REFRESH_URL, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var authResponse2 = await response.Content.ReadFromJsonAsync<RefreshTokenResponse>();

        authResponse2.Should().NotBeNull();
        authResponse2.Token.Should().NotBeNullOrWhiteSpace();
        authResponse2.RefreshToken.Should().NotBeNullOrWhiteSpace();

        authResponse2.Token.Should().NotBe(authResponse1.Token);
        authResponse2.RefreshToken.Should().NotBe(authResponse1.RefreshToken);
    }

    [Fact]
    public async Task RefreshToken_Should_ReturnUnauthorized_WhenOldWorkshopRefreshTokenIsReused()
    {
        // Arrange
        var authResponse1 = await _authHelper.CreateAndLoginWorkshopAsync();
        var request1 = new RefreshTokenRequest(authResponse1.RefreshToken, authResponse1.Token);

        var response2 = await _client.PostAsJsonAsync(REFRESH_URL, request1);
        response2.EnsureSuccessStatusCode();

        // Act
        var response3 = await _client.PostAsJsonAsync(REFRESH_URL, request1);

        // Assert
        response3.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefreshToken_Should_ReturnUnauthorized_WhenWorkshopRefreshTokenIsInvalid()
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginWorkshopAsync();
        var request = new RefreshTokenRequest("to-jest-zly-token-dla-workshopa", authResponse.Token);

        // Act
        var response = await _client.PostAsJsonAsync(REFRESH_URL, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

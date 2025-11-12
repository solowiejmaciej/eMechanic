namespace eMechanic.Integration.Tests.Tokens;

using System.Net;
using System.Net.Http.Json;
using eMechanic.API.Constans;
using eMechanic.API.Features.Tokens;
using eMechanic.API.Features.User;
using eMechanic.API.Features.Workshop;
using eMechanic.Application.Token.Features.Create.User;
using eMechanic.Application.Users.Features.Create;
using eMechanic.Application.Workshop.Features.Create;
using eMechanic.Integration.Tests.TestContainers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

public class CreateUserTokenFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private const string BASE_API_URL = $"/api/{WebApiConstans.CURRENT_API_VERSION}";

    private const string TEST_EMAIL = "user-login@test.com";
    private const string TEST_PASSWORD = "Password123!";

    public CreateUserTokenFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateUserToken_Should_ReturnOkAndToken_WhenCredentialsAreValid()
    {
        // Arrange
        await CreateTestUser(TEST_EMAIL, TEST_PASSWORD);
        var command = new CreateUserTokenCommand(TEST_EMAIL, TEST_PASSWORD);

        // Act
        var response = await _client.PostAsJsonAsync($"{BASE_API_URL}{TokenPrefix.CREATE_USER_TOKEN_ENDPOINT}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var loginResponse = await response.Content.ReadFromJsonAsync<CreateUserTokenResponse>();
        loginResponse.Should().NotBeNull();
        loginResponse!.Token.Should().NotBeNullOrWhiteSpace();
        loginResponse.UserId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateUserToken_Should_ReturnBadRequest_WhenPasswordIsInvalid()
    {
        // Arrange
        await CreateTestUser("email@test.com", TEST_PASSWORD);
        var command = new CreateUserTokenCommand("wrong-pass-user@test.com", "WrongPassword!");

        // Act
        var response = await _client.PostAsJsonAsync($"{BASE_API_URL}{TokenPrefix.CREATE_USER_TOKEN_ENDPOINT}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Errors.Should().ContainKey("General");
        problem.Errors["General"].Should().Contain("Invalid email or password.");
    }

    [Fact]
    public async Task CreateUserToken_Should_ReturnBadRequest_WhenEmailDoesNotExist()
    {
        // Arrange
        var command = new CreateUserTokenCommand("non-existent-user@test.com", TEST_PASSWORD);

        // Act
        var response = await _client.PostAsJsonAsync($"{BASE_API_URL}{TokenPrefix.CREATE_USER_TOKEN_ENDPOINT}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUserToken_Should_ReturnBadRequest_WhenTryingToLoginAsWorkshop()
    {
        // Arrange
        await CreateTestWorkshop("workshop-account@test.com", TEST_PASSWORD);

        var command = new CreateUserTokenCommand("workshop-account@test.com", TEST_PASSWORD);

        // Act
        var response = await _client.PostAsJsonAsync($"{BASE_API_URL}{TokenPrefix.CREATE_USER_TOKEN_ENDPOINT}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Errors["General"].Should().Contain("Invalid email or password.");
    }

    private async Task CreateTestWorkshop(string email, string password)
    {
        var command = new CreateWorkshopCommand(
            email,
            password,
            $"contact-{Guid.NewGuid()}@workshop.com",
            "Super Warsztat",
            $"SuperW-{Guid.NewGuid()}",
            "987654321",
            "ul. Testowa 1",
            "Poznań",
            "60-123",
            "Polska");

        await _client.PostAsJsonAsync($"{BASE_API_URL}{WorkshopPrefix.CREATE_ENDPOINT}", command);
    }
    private async Task CreateTestUser(string email, string password)
    {
        var command = new CreateUserCommand(
            "Jan",
            "Tester",
            email,
            password);

        await _client.PostAsJsonAsync($"{BASE_API_URL}{UserPrefix.CREATE_USER_ENDPOINT}", command);
    }
}

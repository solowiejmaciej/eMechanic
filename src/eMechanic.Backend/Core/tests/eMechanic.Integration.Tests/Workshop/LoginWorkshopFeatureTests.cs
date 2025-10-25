using System.Net;
using System.Net.Http.Json;
using eMechanic.Application.Workshop.Login;
using eMechanic.Integration.Tests.TestContainers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace eMechanic.Integration.Tests.Workshop;

using System.Text.Json;
using Application.Users.Register;
using Application.Workshop.Register;

public class LoginWorkshopFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private const string TEST_EMAIL = "workshop-login@test.com";
    private const string TEST_PASSWORD = "Password123!";

    public LoginWorkshopFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task LoginWorkshop_Should_ReturnOkAndToken_WhenCredentialsAreValid()
    {
        // Arrange
        await CreateTestWorkshop(TEST_EMAIL, TEST_PASSWORD);
        var command = new LoginWorkshopCommand(TEST_EMAIL, TEST_PASSWORD);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/workshops/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginWorkshopResponse>();
        loginResponse.Should().NotBeNull();
        loginResponse!.Token.Should().NotBeNullOrWhiteSpace();
        loginResponse.WorkshopId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task LoginWorkshop_Should_ReturnBadRequest_WhenPasswordIsInvalid()
    {
        // Arrange
        await CreateTestWorkshop("wrong-pass@test.com", TEST_PASSWORD);
        var command = new LoginWorkshopCommand("wrong-pass@test.com", "WrongPassword!");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/workshops/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Errors.Should().ContainKey("General");
        problem.Errors["General"].Should().Contain("Invalid email or password.");
    }

    [Fact]
    public async Task LoginWorkshop_Should_ReturnBadRequest_WhenEmailDoesNotExist()
    {
        // Arrange
        var command = new LoginWorkshopCommand("non-existent@test.com", TEST_PASSWORD);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/workshops/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task LoginWorkshop_Should_ReturnBadRequest_WhenTryingToLoginAsUser()
    {
        // Arrange
        await CreateTestUser("user-account@test.com", TEST_PASSWORD);

        var command = new LoginWorkshopCommand("user-account@test.com", TEST_PASSWORD);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/workshops/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Errors["General"].Should().Contain("Invalid email or password.");
    }

    private async Task CreateTestWorkshop(string email, string password)
    {
        var command = new RegisterWorkshopCommand(
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

        await _client.PostAsJsonAsync("/api/v1/workshops/register", command);
    }
    private async Task CreateTestUser(string email, string password)
    {
        var command = new RegisterUserCommand(
            "Jan",
            "Tester",
            email,
            password);

        await _client.PostAsJsonAsync("/api/v1/users/register", command);
    }

}

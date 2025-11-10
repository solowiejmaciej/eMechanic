using System.Net;
using System.Net.Http.Json;
using eMechanic.Infrastructure.Identity;
using eMechanic.Integration.Tests.TestContainers;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace eMechanic.Integration.Tests.User;

using System.Text.RegularExpressions;
using Application.Users.Features.Login;
using Application.Users.Features.Register;
using Application.Workshop.Features.Register;

public class LoginUserFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;

    private const string TestEmail = "user-login@test.com";
    private const string TestPassword = "Password123!";

    public LoginUserFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task LoginUser_Should_ReturnOkAndToken_WhenCredentialsAreValid()
    {
        // Arrange
        await CreateTestUser(TestEmail, TestPassword);
        var command = new LoginUserCommand(TestEmail, TestPassword);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginUserResponse>();
        loginResponse.Should().NotBeNull();
        loginResponse!.Token.Should().NotBeNullOrWhiteSpace();
        loginResponse.UserId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task LoginUser_Should_ReturnBadRequest_WhenPasswordIsInvalid()
    {
        // Arrange
        await CreateTestUser("email@test.com", TestPassword);
        var command = new LoginUserCommand("wrong-pass-user@test.com", "WrongPassword!");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Errors.Should().ContainKey("General");
        problem.Errors["General"].Should().Contain("Invalid email or password.");
    }

    [Fact]
    public async Task LoginUser_Should_ReturnBadRequest_WhenEmailDoesNotExist()
    {
        // Arrange
        var command = new LoginUserCommand("non-existent-user@test.com", TestPassword);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task LoginUser_Should_ReturnBadRequest_WhenTryingToLoginAsWorkshop()
    {
        // Arrange
        await CreateTestWorkshop("workshop-account@test.com", TestPassword);

        var command = new LoginUserCommand("workshop-account@test.com", TestPassword);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users/login", command);

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

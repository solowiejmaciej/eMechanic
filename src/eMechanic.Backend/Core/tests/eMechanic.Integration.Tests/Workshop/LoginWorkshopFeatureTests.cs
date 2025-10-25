using System.Net;
using System.Net.Http.Json;
using eMechanic.Application.Workshop.Login;
using eMechanic.Infrastructure.Identity;
using eMechanic.Integration.Tests.TestContainers;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace eMechanic.Integration.Tests.Workshop;

using Application.Abstractions.Identity;

public class LoginWorkshopFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly IServiceProvider _serviceProvider;

    private const string TEST_EMAIL = "workshop-login@test.com";
    private const string TEST_PASSWORD = "Password123!";

    public LoginWorkshopFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _serviceProvider = factory.Services;
    }

    private async Task<Guid> CreateTestIdentity(EIdentityType type, string email, string password)
    {
        using var scope = _serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Identity>>();

        var identity = Identity.Create(email, type);
        var result = await userManager.CreateAsync(identity, password);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException("Failed to create test identity: " +
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        return identity.Id;
    }

    [Fact]
    public async Task LoginWorkshop_Should_ReturnOkAndToken_WhenCredentialsAreValid()
    {
        // Arrange
        await CreateTestIdentity(EIdentityType.Workshop, TEST_EMAIL, TEST_PASSWORD);
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
        await CreateTestIdentity(EIdentityType.Workshop, "wrong-pass@test.com", TEST_PASSWORD);
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
        await CreateTestIdentity(EIdentityType.User, "user-account@test.com", TEST_PASSWORD);

        var command = new LoginWorkshopCommand("user-account@test.com", TEST_PASSWORD);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/workshops/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Errors["General"].Should().Contain("Invalid email or password.");
    }
}

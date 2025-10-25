using System.Net;
using System.Net.Http.Json;
using eMechanic.Application.Users.Login;
using eMechanic.Infrastructure.Identity;
using eMechanic.Integration.Tests.TestContainers;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace eMechanic.Integration.Tests.User;

using Application.Abstractions.Identity;

public class LoginUserFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly IServiceProvider _serviceProvider;

    private const string TestEmail = "user-login@test.com";
    private const string TestPassword = "Password123!";

    public LoginUserFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _serviceProvider = factory.Services;
    }

    private async Task<Guid> CreateTestIdentity(EIdentityType type, string email, string password)
    {
        using var scope = _serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Identity>>();

        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            await userManager.DeleteAsync(existingUser);
        }

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
    public async Task LoginUser_Should_ReturnOkAndToken_WhenCredentialsAreValid()
    {
        // Arrange
        await CreateTestIdentity(EIdentityType.User, TestEmail, TestPassword);
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
        await CreateTestIdentity(EIdentityType.User, "wrong-pass-user@test.com", TestPassword);
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
        await CreateTestIdentity(EIdentityType.Workshop, "workshop-account@test.com", TestPassword);

        var command = new LoginUserCommand("workshop-account@test.com", TestPassword);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Errors["General"].Should().Contain("Invalid email or password.");
    }
}

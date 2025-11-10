namespace eMechanic.Integration.Tests.User;

using System.Net;
using System.Net.Http.Json;
using Application.Users.Features.Register;
using FluentAssertions;
using TestContainers;

public class RegisterUserFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;

    public RegisterUserFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RegisterUser_Should_ReturnCreated_WhenDataIsValid()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "Jan",
            "Tester",
            $"test-{Guid.NewGuid()}@integration.com",
            "zaq1@WSX");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task RegisterUser_Should_ReturnBadRequest_WhenEmailIsInvalid()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "Jan",
            "Tester",
            $"this-is-not-a-valid-email",
            "zaq1@WSX");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterUser_Should_ReturnBadRequest_WhenPasswordIsTooShort()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "Jan",
            "Tester",
            $"test-{Guid.NewGuid()}@integration.com",
            "short");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterUser_Should_ReturnBadRequest_WhenEmailAlreadyExists()
    {
        // Arrange
        var email = $"test-{Guid.NewGuid()}@integration.com";
        var command1 = new RegisterUserCommand(
            "Jan",
            "Tester",
            email,
            "zaq1@WSX");

        var command2 = new RegisterUserCommand(
            "Anna",
            "Kowalska",
            email,
            "zaq1@WSX");

        // Act
        var response1 = await _client.PostAsJsonAsync("/api/v1/users/register", command1);
        var response2 = await _client.PostAsJsonAsync("/api/v1/users/register", command2);

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.Created);
        response2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterUser_Should_ReturnBadRequest_WhenFirstNameIsEmpty()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "",
            "Tester",
            $"test-{Guid.NewGuid()}@integration.com",
            "zaq1@WSX");
        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users/register", command);
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

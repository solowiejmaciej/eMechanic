namespace eMechanic.Integration.Tests.User;

using System.Net;
using System.Net.Http.Json;
using Application.Users.Features.Create;
using FluentAssertions;
using TestContainers;

public class CreateUserFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;

    public CreateUserFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateUser_Should_ReturnCreated_WhenDataIsValid()
    {
        // Arrange
        var command = new CreateUserCommand(
            "Jan",
            "Tester",
            $"test-{Guid.NewGuid()}@integration.com",
            "zaq1@WSX");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateUser_Should_ReturnBadRequest_WhenEmailIsInvalid()
    {
        // Arrange
        var command = new CreateUserCommand(
            "Jan",
            "Tester",
            $"this-is-not-a-valid-email",
            "zaq1@WSX");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUser_Should_ReturnBadRequest_WhenPasswordIsTooShort()
    {
        // Arrange
        var command = new CreateUserCommand(
            "Jan",
            "Tester",
            $"test-{Guid.NewGuid()}@integration.com",
            "short");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUser_Should_ReturnBadRequest_WhenEmailAlreadyExists()
    {
        // Arrange
        var email = $"test-{Guid.NewGuid()}@integration.com";
        var command1 = new CreateUserCommand(
            "Jan",
            "Tester",
            email,
            "zaq1@WSX");

        var command2 = new CreateUserCommand(
            "Anna",
            "Kowalska",
            email,
            "zaq1@WSX");

        // Act
        var response1 = await _client.PostAsJsonAsync("/api/v1/users", command1);
        var response2 = await _client.PostAsJsonAsync("/api/v1/users", command2);

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.Created);
        response2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUser_Should_ReturnBadRequest_WhenFirstNameIsEmpty()
    {
        // Arrange
        var command = new CreateUserCommand(
            "",
            "Tester",
            $"test-{Guid.NewGuid()}@integration.com",
            "zaq1@WSX");
        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users", command);
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

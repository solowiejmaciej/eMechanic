namespace eMechanic.Integration.Tests.User;

using System.Net;
using System.Net.Http.Json;
using API.Features.User.Update.Request;
using Application.Token.Features.Create.User;
using Application.Users.Features.Get.Current;
using Common.Result.Fields;
using FluentAssertions;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using TestContainers;

public class UpdateUserFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly AuthHelper _authHelper;

    public UpdateUserFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _authHelper = new AuthHelper(_client);
    }

    [Fact]
    public async Task UpdateUser_Should_ReturnNoContent_And_SuccessfullyUpdateAllData()
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginUserAsync("original@test.com", "Password123!");
        _client.SetBearerToken(authResponse.Token);

        var updateRequest = new UpdateUserRequest("Jan", "Kowalski", "new-email@test.com");

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/users", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync("/api/v1/users/me");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var userDto = await getResponse.Content.ReadFromJsonAsync<GetCurrentUserResponse>();
        userDto.Should().NotBeNull();
        userDto!.Id.Should().Be(authResponse.DomainId);
        userDto.FirstName.Should().Be("Jan");
        userDto.LastName.Should().Be("Kowalski");
        userDto.Email.Should().Be("new-email@test.com");

        _client.ClearBearerToken();
        var loginCmd = new CreateUserTokenCommand("new-email@test.com", "Password123!");
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/tokens/user", loginCmd);

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var loginDto = await loginResponse.Content.ReadFromJsonAsync<CreateUserTokenResponse>();
        loginDto!.UserId.Should().Be(authResponse.DomainId);

        var oldLoginCmd = new CreateUserTokenCommand("original@test.com", "Password123!");
        var oldLoginResponse = await _client.PostAsJsonAsync("/api/v1/tokens/user", oldLoginCmd);
        oldLoginResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task UpdateUser_Should_ReturnBadRequest_WhenEmailIsAlreadyTakenByAnotherUser()
    {
        // Arrange
        await _authHelper.CreateAndLoginUserAsync("taken@test.com");
        _client.ClearBearerToken();

        var authResponse = await _authHelper.CreateAndLoginUserAsync("user-b@test.com");
        _client.SetBearerToken(authResponse.Token);

        var updateRequest = new UpdateUserRequest("UserB", "LastNameB", "taken@test.com");

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/users", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Errors.Should().ContainKey(EField.Email.ToString());
        problem.Errors[EField.Email.ToString()].Should().Contain("Email already in use.");

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task UpdateUser_Should_ReturnUnauthorized_WhenTokenIsMissing()
    {
        // Arrange
        _client.ClearBearerToken();
        var updateRequest = new UpdateUserRequest("Test", "Test", "test@test.com");

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/users", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateUser_Should_ReturnForbidden_WhenWorkshopTokenIsUsed()
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginWorkshopAsync();
        _client.SetBearerToken(authResponse.Token);
        var updateRequest = new UpdateUserRequest("Test", "Test", "test@test.com");

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/users", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        _client.ClearBearerToken();
    }

    [Theory]
    [InlineData("", "Kowalski", "test@test.com", "FirstName")]
    [InlineData("Jan", "", "test@test.com", "LastName")]
    [InlineData("Jan", "Kowalski", "", "Email")]
    [InlineData("Jan", "Kowalski", "zly-email", "Email")]
    public async Task UpdateUser_Should_ReturnBadRequest_WhenValidationFails(
        string firstName, string lastName, string email, string expectedErrorField)
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(authResponse.Token);

        var updateRequest = new UpdateUserRequest(firstName, lastName, email);

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/users", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Errors.Should().ContainKey(expectedErrorField);

        _client.ClearBearerToken();
    }
}

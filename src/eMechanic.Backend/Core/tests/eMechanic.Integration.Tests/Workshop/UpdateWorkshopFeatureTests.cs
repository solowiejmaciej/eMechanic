namespace eMechanic.Integration.Tests.Workshop;

using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using API.Features.Workshop.Update.Request;
using Application.Workshop.Features.Login;
using Common.Result.Fields;
using FluentAssertions;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using TestContainers;

public class UpdateWorkshopFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly AuthHelper _authHelper;

    public UpdateWorkshopFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _authHelper = new AuthHelper(_client);
    }

    private UpdateWorkshopRequest CreateValidRequest(string email) => new(
        email,
        $"contact-new-{Guid.NewGuid()}@w.pl",
        "Nowa Nazwa Warsztatu",
        $"NowyDisplay-{Guid.NewGuid()}",
        "987654321",
        "Nowa Ulica 1",
        "Nowe Miasto",
        "11-222",
        "Nowy Kraj");

    [Fact]
    public async Task UpdateWorkshop_Should_ReturnNoContent_And_SuccessfullyUpdateAllData()
    {
        // Arrange
        var originalEmail = $"original-ws-{Guid.NewGuid()}@int.com";
        var (workshopId, token) = await _authHelper.CreateAndLoginWorkshopAsync(originalEmail, "Password123!");
        _client.SetBearerToken(token);

        var newEmail = $"new-ws-{Guid.NewGuid()}@int.com";
        var updateRequest = CreateValidRequest(newEmail);

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/workshops", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        _client.ClearBearerToken();
        var loginCmd = new LoginWorkshopCommand(newEmail, "Password123!");
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/workshops/login", loginCmd);

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var loginDto = await loginResponse.Content.ReadFromJsonAsync<LoginWorkshopResponse>();
        loginDto!.WorkshopId.Should().Be(workshopId);

        var oldLoginCmd = new LoginWorkshopCommand(originalEmail, "Password123!");
        var oldLoginResponse = await _client.PostAsJsonAsync("/api/v1/workshops/login", oldLoginCmd);
        oldLoginResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task UpdateWorkshop_Should_ReturnForbidden_WhenUserTokenIsUsed()
    {
        // Arrange
        var (_, userToken) = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(userToken);

        var updateRequest = CreateValidRequest("test@test.com");

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/workshops", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        _client.ClearBearerToken();
    }

    [Fact]
    public async Task UpdateWorkshop_Should_ReturnBadRequest_WhenEmailIsTakenByAnotherWorkshop()
    {
        // Arrange
        var takenEmail = $"taken-ws-{Guid.NewGuid()}@int.com";
        await _authHelper.CreateAndLoginWorkshopAsync(takenEmail);
        _client.ClearBearerToken();

        var (_, tokenB) = await _authHelper.CreateAndLoginWorkshopAsync($"workshop-b-{Guid.NewGuid()}@int.com");
        _client.SetBearerToken(tokenB);

        var updateRequest = CreateValidRequest(takenEmail);

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/workshops", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Errors.Should().ContainKey(EField.Email.ToString());
        problem.Errors[EField.Email.ToString()].Should().Contain("Email (login) already in use.");

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task UpdateWorkshop_Should_ReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var (_, token) = await _authHelper.CreateAndLoginWorkshopAsync();
        _client.SetBearerToken(token);

        var updateRequest = CreateValidRequest("new@email.com") with { Name = "" };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/workshops", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Errors.Should().ContainKey("Name");

        _client.ClearBearerToken();
    }
}

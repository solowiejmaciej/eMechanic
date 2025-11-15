namespace eMechanic.Integration.Tests.Vehicle;

using System.Net;
using System.Net.Http.Json;
using API.Constans;
using API.Features.Vehicle;
using API.Features.Vehicle.Vehicle.Create.Request;
using Domain.Vehicle.Enums;
using FluentAssertions;
using Helpers;
using TestContainers;

public class DeleteVehicleFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly AuthHelper _authHelper;
    private const string BASE_API_URL = $"/api/{WebApiConstans.CURRENT_API_VERSION}{VehiclePrefix.DELETE}";
    public DeleteVehicleFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _authHelper = new AuthHelper(_client);
    }

    private async Task<(Guid UserId, Guid VehicleId, string Token)> CreateVehicleForTestUser(string suffix = "delete")
    {
        var authResponse = await _authHelper.CreateAndLoginUserAsync($"user-{suffix}-{Guid.NewGuid()}@int.com");
        _client.SetBearerToken(authResponse.Token);

        var createRequest = new CreateVehicleRequest(
            $"V1N{Guid.NewGuid().ToString("N")[..14]}",
            $"DeleteTest Manufacturer {suffix}",
            $"DeleteTest Model {suffix}",
            "2019",
            1.2m,
            200,
            EMileageUnit.Miles,
            "PZ1W924",
            124,
            EFuelType.Gasoline,
            EBodyType.Sedan,
            EVehicleType.Passenger);
        var createResponse = await _client.PostAsJsonAsync("/api/v1/vehicles", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var createdContent = await createResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var vehicleId = createdContent!["vehicleId"];

        return (authResponse.DomainId, vehicleId, authResponse.Token);
    }

    [Fact]
    public async Task DeleteVehicle_Should_ReturnNoContent_WhenVehicleExistsForUser()
    {
        // Arrange
        var (_, vehicleId, token) = await CreateVehicleForTestUser();
        _client.SetBearerToken(token);

        // Act
        var response = await _client.DeleteAsync($"{BASE_API_URL.Replace("{id:guid}", vehicleId.ToString())}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"{BASE_API_URL}{vehicleId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task DeleteVehicle_Should_ReturnNotFound_WhenVehicleDoesNotExist()
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(authResponse.Token);
        var nonExistentVehicleId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"{BASE_API_URL.Replace("{id:guid}", nonExistentVehicleId.ToString())}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task DeleteVehicle_Should_ReturnNotFound_WhenVehicleBelongsToAnotherUser()
    {
        // Arrange
        var (_, vehicleId, _) = await CreateVehicleForTestUser("ownerA");
        _client.ClearBearerToken();

        var authResponse = await _authHelper.CreateAndLoginUserAsync("ownerB@gmail.com");
        _client.SetBearerToken(authResponse.Token);

        // Act
        var response = await _client.DeleteAsync($"{BASE_API_URL.Replace("{id:guid}", vehicleId.ToString())}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        _client.ClearBearerToken();
    }


    [Fact]
    public async Task DeleteVehicle_Should_ReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var (_, vehicleId, _) = await CreateVehicleForTestUser();
        _client.ClearBearerToken();

        // Act
        var response = await _client.DeleteAsync($"{BASE_API_URL.Replace("{id:guid}", vehicleId.ToString())}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteVehicle_Should_ReturnForbiddenOrUnauthorized_WhenWorkshopTokenIsUsed()
    {
        // Arrange
        var (_, vehicleId, _) = await CreateVehicleForTestUser();
        _client.ClearBearerToken();

        var authResponse = await _authHelper.CreateAndLoginWorkshopAsync();
        _client.SetBearerToken(authResponse.Token);

        // Act
        var response = await _client.DeleteAsync($"{BASE_API_URL.Replace("{id:guid}", vehicleId.ToString())}");

        // Assert
        response.StatusCode.Should().Match(code =>
            code == HttpStatusCode.Unauthorized || code == HttpStatusCode.Forbidden,
             "Workshop token should not allow deleting user's vehicle");

        _client.ClearBearerToken();
    }
}

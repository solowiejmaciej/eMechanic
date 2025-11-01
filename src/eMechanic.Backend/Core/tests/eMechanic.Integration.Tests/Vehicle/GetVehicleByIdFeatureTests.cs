namespace eMechanic.Integration.Tests.Vehicle;

using System.Net;
using System.Net.Http.Json;
using API.Features.Vehicle.Create.Request;
using Application.Vehicle.Get;
using Domain.Vehicle.Enums;
using FluentAssertions;
using Helpers;
using TestContainers;


public class GetVehicleByIdFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly AuthHelper _authHelper;

    public GetVehicleByIdFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _authHelper = new AuthHelper(_client);
    }

     private async Task<(Guid UserId, Guid VehicleId, string Token)> CreateVehicleForTestUser()
     {
         var (userId, token) = await _authHelper.CreateAndLoginUserAsync();
         _client.SetBearerToken(token);

         var createRequest = new CreateVehicleRequest(
             $"V1N{Guid.NewGuid().ToString("N")[..14]}", "GetById Manufacturer", "GetById Model", "2021",
             1.4m, 200, EMileageUnit.Kilometers, EFuelType.Gasoline, EBodyType.Hatchback, EVehicleType.Passenger);
         var createResponse = await _client.PostAsJsonAsync("/api/v1/vehicles", createRequest);
         createResponse.EnsureSuccessStatusCode();
         var createdContent = await createResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
         var vehicleId = createdContent!["vehicleId"];

         return (userId, vehicleId, token);
     }

    [Fact]
    public async Task GetVehicleById_Should_ReturnOkAndVehicle_WhenVehicleExistsForUser()
    {
        // Arrange
        var (_, vehicleId, token) = await CreateVehicleForTestUser();
        _client.SetBearerToken(token);

        // Act
        var response = await _client.GetAsync($"/api/v1/vehicles/{vehicleId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var vehicleResponse = await response.Content.ReadFromJsonAsync<VehicleResponse>();
        vehicleResponse.Should().NotBeNull();
        vehicleResponse!.Id.Should().Be(vehicleId);
        vehicleResponse.Manufacturer.Should().Be("GetById Manufacturer");

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task GetVehicleById_Should_ReturnNotFound_WhenVehicleDoesNotExist()
    {
        // Arrange
        var (_, token) = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(token);
        var nonExistentVehicleId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/vehicles/{nonExistentVehicleId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        _client.ClearBearerToken();
    }

     [Fact]
    public async Task GetVehicleById_Should_ReturnNotFound_WhenVehicleExistsButBelongsToAnotherUser()
    {
        // Arrange
        var (_, vehicleId, _) = await CreateVehicleForTestUser();
        _client.ClearBearerToken();

         var (_, otherUserToken) = await _authHelper.CreateAndLoginUserAsync();
         _client.SetBearerToken(otherUserToken);

        // Act
        var response = await _client.GetAsync($"/api/v1/vehicles/{vehicleId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task GetVehicleById_Should_ReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var (_, vehicleId, _) = await CreateVehicleForTestUser();
        _client.ClearBearerToken();

        // Act
        var response = await _client.GetAsync($"/api/v1/vehicles/{vehicleId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

     [Fact]
    public async Task GetVehicleById_Should_ReturnForbiddenOrUnauthorized_WhenWorkshopTokenIsUsed()
    {
        // Arrange
        var (_, vehicleId, _) = await CreateVehicleForTestUser();
        _client.ClearBearerToken();

        var (_, workshopToken) = await _authHelper.CreateAndLoginWorkshopAsync();
        _client.SetBearerToken(workshopToken);

        // Act
        var response = await _client.GetAsync($"/api/v1/vehicles/{vehicleId}");

        // Assert
        response.StatusCode.Should().Match(code =>
            code == HttpStatusCode.Unauthorized || code == HttpStatusCode.Forbidden || code == HttpStatusCode.NotFound,
             "Workshop token should not allow getting user's vehicle");

        _client.ClearBearerToken();
    }
}


namespace eMechanic.Integration.Tests.RepairRequest;

using System.Net;
using System.Net.Http.Json;
using API.Constans;
using API.Features.RepairRequest;
using Application.RepairRequest.Features.Create;
using Domain.Vehicle.Enums;
using eMechanic.API.Features.Vehicle.Vehicle.Create.Request;
using FluentAssertions;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using TestContainers;

[Collection("Sequential")]
public class CreateRepairRequestTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly AuthHelper _authHelper;
    private const string BASE_API_URL = $"/api/{WebApiConstans.CURRENT_API_VERSION}";


    public CreateRepairRequestTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _authHelper = new AuthHelper(_client);
    }

    private async Task<(Guid vehicleId, Guid workshopId)> CreateVehicleAndWorkshopAsync(string userToken)
    {
        _client.SetBearerToken(userToken);

        var vehicleRequest = new CreateVehicleRequest(
            $"V1N{Guid.NewGuid().ToString("N")[..14]}", "Test", "Model", "2022", 2.0m, 10000,
            EMileageUnit.Kilometers, "XYZ123", 150, EFuelType.Gasoline, EBodyType.Sedan, EVehicleType.Passenger);
        var vehicleResponse = await _client.PostAsJsonAsync("/api/v1/vehicles", vehicleRequest);
        vehicleResponse.EnsureSuccessStatusCode();
        var vehicleContent = await vehicleResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var vehicleId = vehicleContent!["vehicleId"];

        // Create Workshop
        var workshopAuth = await _authHelper.CreateAndLoginWorkshopAsync();
        var workshopId = workshopAuth.DomainId;

        return (vehicleId, workshopId);
    }

    [Fact]
    public async Task CreateRepairRequest_ShouldReturnCreated_WhenDataIsValid()
    {
        // Arrange
        var userAuth = await _authHelper.CreateAndLoginUserAsync();
        var (vehicleId, workshopId) = await CreateVehicleAndWorkshopAsync(userAuth.Token);
        var command = new CreateRepairRequestCommand(vehicleId, workshopId, "Fix my car!");

        // Act
        var response = await _client.PostAsJsonAsync($"{BASE_API_URL}{RepairRequestPrefix.CREATE}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        content!["repairRequestId"].Should().NotBeEmpty();
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateRepairRequest_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _client.ClearBearerToken();
        var command = new CreateRepairRequestCommand(Guid.NewGuid(), Guid.NewGuid(), "Fix my car!");

        // Act
        var response = await _client.PostAsJsonAsync($"{BASE_API_URL}{RepairRequestPrefix.CREATE}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateRepairRequest_ShouldReturnNotFound_WhenUserDoesNotOwnVehicle()
    {
        // Arrange
        var userAuth1 = await _authHelper.CreateAndLoginUserAsync();
        var (_, workshopId) = await CreateVehicleAndWorkshopAsync(userAuth1.Token);

        var userAuth2 = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(userAuth2.Token);
        var command = new CreateRepairRequestCommand(Guid.NewGuid(), workshopId, "Fix my car!");

        // Act
        var response = await _client.PostAsJsonAsync($"{BASE_API_URL}{RepairRequestPrefix.CREATE}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateRepairRequest_ShouldReturnBadRequest_WhenDescriptionIsEmpty()
    {
        // Arrange
        var userAuth = await _authHelper.CreateAndLoginUserAsync();
        var (vehicleId, workshopId) = await CreateVehicleAndWorkshopAsync(userAuth.Token);
        var command = new CreateRepairRequestCommand(vehicleId, workshopId, string.Empty);

        // Act
        var response = await _client.PostAsJsonAsync($"{BASE_API_URL}{RepairRequestPrefix.CREATE}", command);


        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("Description");
    }
}

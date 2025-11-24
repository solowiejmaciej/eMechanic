namespace eMechanic.Integration.Tests.RepairRequest;

using System.Net;
using System.Net.Http.Json;
using API.Constans;
using API.Features.RepairRequest;
using API.Features.Vehicle;
using Application.RepairRequest.Features.Create;
using Application.RepairRequest.Features.Get;
using Common.Result;
using Domain.Vehicle.Vehicle.Enums;
using eMechanic.API.Features.Vehicle.Vehicle.Create.Request;
using FluentAssertions;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using TestContainers;
using Xunit;

[Collection("Sequential")]
public class GetRepairRequestFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly AuthHelper _authHelper;
    private const string BASE_API_URL = $"/api/{WebApiConstans.CURRENT_API_VERSION}";

    public GetRepairRequestFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _authHelper = new AuthHelper(_client);
    }

    private async Task<(Guid vehicleId, Guid workshopId, Guid repairRequestId, string userToken, string workshopToken)> SetupRepairRequestScenarioAsync()
    {
        // 1. Create User & Vehicle
        var userAuth = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(userAuth.Token);

        var vehicleRequest = new CreateVehicleRequest(
            $"V1N{Guid.NewGuid().ToString("N")[..14]}", "Mazda", "6", "2020", 2.0m, 50000,
            EMileageUnit.Kilometers, "PO 12345", 165, EFuelType.Gasoline, EBodyType.Sedan, EVehicleType.Passenger);

        var vehicleResponse = await _client.PostAsJsonAsync($"{BASE_API_URL}{VehiclePrefix.CREATE}", vehicleRequest);
        vehicleResponse.EnsureSuccessStatusCode();
        var vehicleContent = await vehicleResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var vehicleId = vehicleContent!["vehicleId"];

        // 2. Create Workshop
        var workshopAuth = await _authHelper.CreateAndLoginWorkshopAsync();

        // 3. Create Repair Request (as User)
        _client.SetBearerToken(userAuth.Token);
        var command = new CreateRepairRequestCommand(vehicleId, workshopAuth.DomainId, "Clutch slipping");
        var repairResponse = await _client.PostAsJsonAsync($"{BASE_API_URL}{RepairRequestPrefix.CREATE}", command);
        repairResponse.EnsureSuccessStatusCode();
        var repairContent = await repairResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var repairRequestId = repairContent!["repairRequestId"];

        return (vehicleId, workshopAuth.DomainId, repairRequestId, userAuth.Token, workshopAuth.Token);
    }

    [Fact]
    public async Task GetRepairRequestsForUserVehicle_Should_ReturnList_WhenUserIsOwner()
    {
        // Arrange
        var (vehicleId, _, _, userToken, _) = await SetupRepairRequestScenarioAsync();
        _client.SetBearerToken(userToken);

        var url = $"{BASE_API_URL}{RepairRequestPrefix.GET_BY_VEHICLE_ID.Replace("{vehicleId}", vehicleId.ToString())}?pageNumber=1&pageSize=10";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginationResult<RepairRequestResponse>>();
        result.Should().NotBeNull();
        result!.Items.Should().HaveCountGreaterThanOrEqualTo(1);
        result.Items.First().Description.Should().Be("Clutch slipping");
    }

    [Fact]
    public async Task GetRepairRequestsForWorkshop_Should_ReturnList_WhenAuthenticatedAsWorkshop()
    {
        // Arrange
        var (_, _, _, _, workshopToken) = await SetupRepairRequestScenarioAsync();
        _client.SetBearerToken(workshopToken);

        var url = $"{BASE_API_URL}{RepairRequestPrefix.GET_BY_WORKSHOP_ID}?pageNumber=1&pageSize=10";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginationResult<RepairRequestResponse>>();
        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetRepairRequestsForWorkshop_Should_ReturnForbidden_WhenAuthenticatedAsUser()
    {
        // Arrange
        var (_, _, _, userToken, _) = await SetupRepairRequestScenarioAsync();
        _client.SetBearerToken(userToken);

        var url = $"{BASE_API_URL}{RepairRequestPrefix.GET_BY_WORKSHOP_ID}?pageNumber=1&pageSize=10";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetRepairRequestsForUser_Should_ReturnForbidden_WhenAuthenticatedAsWorkshop()
    {
        // Arrange
        var (_, _, _, _, workshopToken) = await SetupRepairRequestScenarioAsync();
        _client.SetBearerToken(workshopToken);
        var url = $"{BASE_API_URL}{RepairRequestPrefix.GET_BY_VEHICLE_ID.Replace("{vehicleId}", Guid.NewGuid().ToString())}?pageNumber=1&pageSize=10";
        // Act
        var response = await _client.GetAsync(url);
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

}

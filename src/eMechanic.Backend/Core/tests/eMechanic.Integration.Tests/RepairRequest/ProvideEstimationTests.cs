namespace eMechanic.Integration.Tests.RepairRequest;

using System.Net;
using System.Net.Http.Json;
using API.Constans;
using API.Features.RepairRequest;
using API.Features.RepairRequest.ProvideEstimation;
using API.Features.Vehicle;
using Application.RepairRequest.Features.Create;
using Domain.Vehicle.Enums;
using eMechanic.API.Features.Vehicle.Vehicle.Create.Request;
using FluentAssertions;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using TestContainers;

[Collection("Sequential")]
public class ProvideEstimationTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly AuthHelper _authHelper;
    private const string BASE_API_URL = $"/api/{WebApiConstans.CURRENT_API_VERSION}";

    public ProvideEstimationTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _authHelper = new AuthHelper(_client);
    }

    private async Task<Guid> CreateRepairRequestAsync()
    {
        var userAuth = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(userAuth.Token);

        var vehicleRequest = new CreateVehicleRequest(
            $"V1N{Guid.NewGuid().ToString("N")[..14]}", "Test", "Model", "2022", 2.0m, 10000,
            EMileageUnit.Kilometers, "XYZ123", 150, EFuelType.Gasoline, EBodyType.Sedan, EVehicleType.Passenger);

        var vehicleResponse = await _client.PostAsJsonAsync($"{BASE_API_URL}{VehiclePrefix.CREATE}", vehicleRequest);
        var vehicleContent = await vehicleResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var vehicleId = vehicleContent!["vehicleId"];

        var workshopAuth = await _authHelper.CreateAndLoginWorkshopAsync();

        _client.SetBearerToken(userAuth.Token);
        var command = new CreateRepairRequestCommand(vehicleId, workshopAuth.DomainId, "Fix my car!");

        var repairResponse = await _client.PostAsJsonAsync($"{BASE_API_URL}{RepairRequestPrefix.CREATE}", command);
        var repairContent = await repairResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();

        _client.SetBearerToken(workshopAuth.Token);
        return repairContent!["repairRequestId"];
    }

    [Fact]
    public async Task ProvideEstimation_ShouldReturnNoContent_WhenDataIsValid()
    {
        // Arrange
        var repairRequestId = await CreateRepairRequestAsync();
        var request = new ProvideRepairEstimationRequest("Engine failure", 1500.50m, "PLN");

        var url = $"{BASE_API_URL}{RepairRequestPrefix.PROVIDE_ESTIMATION.Replace("{id}", repairRequestId.ToString())}";

        // Act
        var response = await _client.PutAsJsonAsync(url, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ProvideEstimation_ShouldReturnUnauthorized_WhenTokenIsMissing()
    {
        // Arrange
        _client.ClearBearerToken();
        var request = new ProvideRepairEstimationRequest("Engine failure", 1500.50m, "PLN");

        var url = $"{BASE_API_URL}{RepairRequestPrefix.PROVIDE_ESTIMATION.Replace("{id}", Guid.NewGuid().ToString())}";

        // Act
        var response = await _client.PutAsJsonAsync(url, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProvideEstimation_ShouldReturnForbidden_WhenTokenIsFromUser()
    {
        // Arrange
        var repairRequestId = await CreateRepairRequestAsync();
        var userAuth = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(userAuth.Token);
        var request = new ProvideRepairEstimationRequest("Engine failure", 1500.50m, "PLN");

        var url = $"{BASE_API_URL}{RepairRequestPrefix.PROVIDE_ESTIMATION.Replace("{id}", repairRequestId.ToString())}";

        // Act
        var response = await _client.PutAsJsonAsync(url, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ProvideEstimation_ShouldReturnBadRequest_WhenDiagnosisIsEmpty()
    {
        // Arrange
        var repairRequestId = await CreateRepairRequestAsync();
        var request = new ProvideRepairEstimationRequest("", 1500.50m, "PLN");

        var url = $"{BASE_API_URL}{RepairRequestPrefix.PROVIDE_ESTIMATION.Replace("{id}", repairRequestId.ToString())}";

        // Act
        var response = await _client.PutAsJsonAsync(url, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("Diagnosis");
    }
}

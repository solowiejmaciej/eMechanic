
namespace eMechanic.Integration.Tests.RepairRequest;

using System.Net;
using System.Net.Http.Json;
using API.Constans;
using API.Features.RepairRequest;
using API.Features.RepairRequest.ProvideEstimation;
using Application.RepairRequest.Features.Create;
using Domain.Vehicle.Enums;
using eMechanic.API.Features.Vehicle.Vehicle.Create.Request;
using FluentAssertions;
using Helpers;
using TestContainers;

[Collection("Sequential")]
public class EstimationResponseTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly AuthHelper _authHelper;
    private const string BASE_API_URL = $"/api/{WebApiConstans.CURRENT_API_VERSION}";

    public EstimationResponseTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _authHelper = new AuthHelper(_client);
    }

    private async Task<(Guid repairRequestId, string userToken)> CreateAndEstimateRequestAsync()
    {
        var userAuth = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(userAuth.Token);

        var vehicleRequest = new CreateVehicleRequest(
            $"V1N{Guid.NewGuid().ToString("N")[..14]}", "Test", "Model", "2022", 2.0m, 10000,
            EMileageUnit.Kilometers, "XYZ123", 150, EFuelType.Gasoline, EBodyType.Sedan, EVehicleType.Passenger);
        var vehicleResponse = await _client.PostAsJsonAsync("/api/v1/vehicles", vehicleRequest);
        var vehicleContent = await vehicleResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var vehicleId = vehicleContent!["vehicleId"];

        var workshopAuth = await _authHelper.CreateAndLoginWorkshopAsync();

        _client.SetBearerToken(userAuth.Token);
        var createCommand = new CreateRepairRequestCommand(vehicleId, workshopAuth.DomainId, "Fix my car!");
        var repairResponse = await _client.PostAsJsonAsync($"{BASE_API_URL}{RepairRequestPrefix.CREATE}", createCommand);
        var repairContent = await repairResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var repairRequestId = repairContent!["repairRequestId"];

        _client.SetBearerToken(workshopAuth.Token);
        var estimationRequest = new ProvideRepairEstimationRequest("Engine failure", 1500.50m, "PLN");
        await _client.PutAsJsonAsync($"{BASE_API_URL}{RepairRequestPrefix.PROVIDE_ESTIMATION.Replace("{id}", repairRequestId.ToString())}", estimationRequest);

        return (repairRequestId, userAuth.Token);
    }

    [Fact]
    public async Task AcceptEstimation_ShouldReturnNoContent_WhenUserIsOwner()
    {
        // Arrange
        var (repairRequestId, userToken) = await CreateAndEstimateRequestAsync();
        _client.SetBearerToken(userToken);

        // Act
        var response = await _client.PutAsync($"{BASE_API_URL}{RepairRequestPrefix.ACCEPT_ESTIMATION.Replace("{id}", repairRequestId.ToString())}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task RejectEstimation_ShouldReturnNoContent_WhenUserIsOwner()
    {
        // Arrange
        var (repairRequestId, userToken) = await CreateAndEstimateRequestAsync();
        _client.SetBearerToken(userToken);

        // Act
        var requestBody = new { Reason = "Too expensive" };
        var response = await _client.PutAsJsonAsync($"{BASE_API_URL}{RepairRequestPrefix.REJECT_ESTIMATION.Replace("{id}", repairRequestId.ToString())}", requestBody);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task AcceptEstimation_ShouldReturnForbidden_WhenTokenIsFromWorkshop()
    {
        // Arrange
        var (repairRequestId, _) = await CreateAndEstimateRequestAsync();

        // Act
        var response = await _client.PutAsync($"{BASE_API_URL}{RepairRequestPrefix.ACCEPT_ESTIMATION.Replace("{id}", repairRequestId.ToString())}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}

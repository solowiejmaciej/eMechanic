namespace eMechanic.Integration.Tests.RepairRequest;

using System.Net;
using System.Net.Http.Json;
using eMechanic.API.Constans;
using eMechanic.API.Features.RepairRequest;
using eMechanic.API.Features.Vehicle;
using eMechanic.Application.RepairRequest.Features.Create;
using eMechanic.Domain.Vehicle.Enums;
using eMechanic.API.Features.Vehicle.Vehicle.Create.Request;
using FluentAssertions;
using Helpers;
using TestContainers;
using Xunit;

[Collection("Sequential")]
public class SummarizeRepairRequestFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly AuthHelper _authHelper;
    private const string BASE_API_URL = $"/api/{WebApiConstans.CURRENT_API_VERSION}";

    public SummarizeRepairRequestFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _authHelper = new AuthHelper(_client);
    }

    [Fact]
    public async Task SummarizeRepairRequest_Should_ReturnOk_WhenUserIsOwner()
    {
        // Arrange
        var userAuth = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(userAuth.Token);

        // Create Vehicle
        var vehicleRequest = new CreateVehicleRequest(
            $"V1N{Guid.NewGuid().ToString("N")[..14]}", "Audi", "A4", "2018", 2.0m, 120000,
            EMileageUnit.Kilometers, "WA 99999", 190, EFuelType.Diesel, EBodyType.Kombi, EVehicleType.Passenger);

        var vehicleResp = await _client.PostAsJsonAsync($"{BASE_API_URL}{VehiclePrefix.CREATE}", vehicleRequest);
        vehicleResp.EnsureSuccessStatusCode();
        var vContent = await vehicleResp.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var vehicleId = vContent!["vehicleId"];

        // Create Workshop
        var workshopAuth = await _authHelper.CreateAndLoginWorkshopAsync();

        // Create Request
        _client.SetBearerToken(userAuth.Token);
        var createCmd = new CreateRepairRequestCommand(vehicleId, workshopAuth.DomainId, "Strange noise from engine when accelerating.");
        var repairResp = await _client.PostAsJsonAsync($"{BASE_API_URL}{RepairRequestPrefix.CREATE}", createCmd);
        repairResp.EnsureSuccessStatusCode();
        var rContent = await repairResp.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var repairRequestId = rContent!["repairRequestId"];

        var url = $"{BASE_API_URL}{RepairRequestPrefix.SUMMARIZE.Replace("{id}", repairRequestId.ToString())}";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var summary = await response.Content.ReadFromJsonAsync<string>();

        summary.Should().NotBeNullOrEmpty();
        summary.Should().Be("AI Generated Summary Test Content");
    }

    [Fact]
    public async Task SummarizeRepairRequest_Should_ReturnNotFound_WhenRequestDoesNotBelongToUser()
    {
        // Arrange
        var ownerAuth = await _authHelper.CreateAndLoginUserAsync();
        var otherUserAuth = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(otherUserAuth.Token);

        var randomId = Guid.NewGuid();
        var url = $"{BASE_API_URL}{RepairRequestPrefix.SUMMARIZE.Replace("{id}", randomId.ToString())}";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

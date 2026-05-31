namespace eMechanic.Integration.Tests.Repair;

using System.Net;
using System.Net.Http.Json;
using API.Constans;
using API.Features.Repair;
using API.Features.RepairRequest;
using API.Features.RepairRequest.ProvideEstimation;
using API.Features.Vehicle;
using Application.RepairRequest.Features.Create;
using Domain.Repair.Enums;
using eMechanic.API.Features.Vehicle.Vehicle.Create.Request;
using FluentAssertions;
using Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.DAL;
using TestContainers;

[Collection("Sequential")]
public class PayForRepairFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly HttpClient _client;
    private readonly AuthHelper _authHelper;
    private const string BASE_API_URL = $"/api/{WebApiConstans.CURRENT_API_VERSION}";

    public PayForRepairFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _authHelper = new AuthHelper(_client);
    }

    [Fact(Skip = "Flaky test, needs investigation")]
    public async Task PayForRepair_Should_CreateRepairOnAcceptAndMarkAsPaid_WhenFlowIsSuccessful()
    {
        // Arrange
        var userAuth = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(userAuth.Token);

        var vehicleRequest = new CreateVehicleRequest(
            $"V1N{Guid.NewGuid().ToString("N")[..14]}", "Mazda", "6", "2020", 2.0m, 50000,
            Domain.Vehicle.Vehicle.Enums.EMileageUnit.Kilometers,
            "PO12345",
            165,
            Domain.Vehicle.Vehicle.Enums.EFuelType.Gasoline,
            Domain.Vehicle.Vehicle.Enums.EBodyType.Sedan,
            Domain.Vehicle.Vehicle.Enums.EVehicleType.Passenger);

        var vehicleResponse = await _client.PostAsJsonAsync($"{BASE_API_URL}{VehiclePrefix.CREATE}", vehicleRequest);
        vehicleResponse.EnsureSuccessStatusCode();
        var vehicleContent = await vehicleResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var vehicleId = vehicleContent!["vehicleId"];

        var workshopAuth = await _authHelper.CreateAndLoginWorkshopAsync();

        _client.SetBearerToken(userAuth.Token);
        var createRequestCommand = new CreateRepairRequestCommand(vehicleId, workshopAuth.DomainId, "Engine knocking");
        var createRepairRequestResponse = await _client.PostAsJsonAsync($"{BASE_API_URL}{RepairRequestPrefix.CREATE}", createRequestCommand);
        createRepairRequestResponse.EnsureSuccessStatusCode();
        var createRepairRequestContent = await createRepairRequestResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var repairRequestId = createRepairRequestContent!["repairRequestId"];

        _client.SetBearerToken(workshopAuth.Token);
        var provideEstimationRequest = new ProvideRepairEstimationRequest("Diagnosis", 2000m, "PLN");
        var provideEstimationUrl = $"{BASE_API_URL}{RepairRequestPrefix.PROVIDE_ESTIMATION.Replace("{id}", repairRequestId.ToString())}";
        var provideEstimationResponse = await _client.PutAsJsonAsync(provideEstimationUrl, provideEstimationRequest);
        provideEstimationResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        _client.SetBearerToken(userAuth.Token);
        var acceptUrl = $"{BASE_API_URL}{RepairRequestPrefix.ACCEPT_ESTIMATION.Replace("{id}", repairRequestId.ToString())}";
        var acceptResponse = await _client.PutAsync(acceptUrl, null);
        acceptResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        Guid repairId;
        using (var scope = _factory.Services.CreateScope())
        {
            var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var createdRepair = await appDbContext.Repairs.SingleAsync(r => r.RepairRequestId == repairRequestId);
            createdRepair.Status.Should().Be(ERepairStatus.Scheduled);

            var startResult = createdRepair.StartRepair();
            startResult.IsSuccess.Should().BeTrue();
            var completeResult = createdRepair.CompleteRepair(Domain.Shared.ValueObjects.Money.Create(2200m).Value!);
            completeResult.IsSuccess.Should().BeTrue();

            await appDbContext.SaveChangesAsync();
            repairId = createdRepair.Id;
        }

        var payUrl = $"{BASE_API_URL}{RepairPrefix.PAY.Replace("{id}", repairId.ToString())}";

        // Act
        var payResponse = await _client.PutAsync(payUrl, null);

        // Assert
        payResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var assertScope = _factory.Services.CreateScope();
        var assertDbContext = assertScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var paidRepair = await assertDbContext.Repairs.SingleAsync(r => r.Id == repairId);
        paidRepair.Status.Should().Be(ERepairStatus.Paid);
    }
}




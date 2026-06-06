namespace eMechanic.Integration.Tests.Repair;

using System.Net;
using System.Net.Http.Json;
using API.Constans;
using API.Features.RepairRequest;
using API.Features.RepairRequest.ProvideEstimation;
using API.Features.Vehicle;
using Application.RepairRequest.Features.Create;
using Domain.Repair.Enums;
using eMechanic.API;
using eMechanic.API.Features.Vehicle.Vehicle.Create.Request;
using FluentAssertions;
using Helpers;
using Infrastructure.DAL;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestContainers;

internal static class RepairScenarioHelper
{
    private const string BASE_API_URL = $"/api/{WebApiConstans.CURRENT_API_VERSION}";

    internal static async Task<RepairScenario> CreateScheduledRepairAsync(
        HttpClient client,
        WebApplicationFactory<Program> factory)
    {
        var authHelper = new AuthHelper(client);

        var userAuth = await authHelper.CreateAndLoginUserAsync();
        client.SetBearerToken(userAuth.Token);

        var vehicleRequest = new CreateVehicleRequest(
            $"V1N{Guid.NewGuid().ToString("N")[..14]}", "Mazda", "6", "2020", 2.0m, 50000,
            Domain.Vehicle.Vehicle.Enums.EMileageUnit.Kilometers,
            "PO12345",
            165,
            Domain.Vehicle.Vehicle.Enums.EFuelType.Gasoline,
            Domain.Vehicle.Vehicle.Enums.EBodyType.Sedan,
            Domain.Vehicle.Vehicle.Enums.EVehicleType.Passenger);

        var vehicleResponse = await client.PostAsJsonAsync($"{BASE_API_URL}{VehiclePrefix.CREATE}", vehicleRequest);
        vehicleResponse.EnsureSuccessStatusCode();
        var vehicleContent = await vehicleResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var vehicleId = vehicleContent!["vehicleId"];

        var workshopAuth = await authHelper.CreateAndLoginWorkshopAsync();

        client.SetBearerToken(userAuth.Token);
        var createRequestCommand = new CreateRepairRequestCommand(vehicleId, workshopAuth.DomainId, "Engine knocking");
        var createRepairRequestResponse = await client.PostAsJsonAsync($"{BASE_API_URL}{RepairRequestPrefix.CREATE}", createRequestCommand);
        createRepairRequestResponse.EnsureSuccessStatusCode();
        var createRepairRequestContent = await createRepairRequestResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var repairRequestId = createRepairRequestContent!["repairRequestId"];

        client.SetBearerToken(workshopAuth.Token);
        var provideEstimationRequest = new ProvideRepairEstimationRequest("Diagnosis", 2000m, "PLN");
        var provideEstimationUrl = $"{BASE_API_URL}{RepairRequestPrefix.PROVIDE_ESTIMATION.Replace("{id}", repairRequestId.ToString())}";
        var provideEstimationResponse = await client.PutAsJsonAsync(provideEstimationUrl, provideEstimationRequest);
        provideEstimationResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        client.SetBearerToken(userAuth.Token);
        var acceptUrl = $"{BASE_API_URL}{RepairRequestPrefix.ACCEPT_ESTIMATION.Replace("{id}", repairRequestId.ToString())}";
        var acceptResponse = await client.PutAsync(acceptUrl, null);
        acceptResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var scope = factory.Services.CreateScope();
        var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var repair = await appDbContext.Repairs.SingleAsync(r => r.RepairRequestId == repairRequestId);
        repair.Status.Should().Be(ERepairStatus.Scheduled);

        return new RepairScenario(repair.Id, repairRequestId, vehicleId, userAuth.Token, workshopAuth.Token);
    }
}

internal sealed record RepairScenario(
    Guid RepairId,
    Guid RepairRequestId,
    Guid VehicleId,
    string UserToken,
    string WorkshopToken);


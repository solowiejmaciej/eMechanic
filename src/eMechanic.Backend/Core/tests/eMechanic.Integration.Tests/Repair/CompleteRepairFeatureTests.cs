namespace eMechanic.Integration.Tests.Repair;

using System.Net;
using System.Net.Http.Json;
using API.Constans;
using API.Features.Repair;
using API.Features.Repair.Complete;
using Domain.Repair.Enums;
using FluentAssertions;
using Helpers;
using Infrastructure.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestContainers;

[Collection("Sequential")]
public class CompleteRepairFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly HttpClient _client;
    private const string BASE_API_URL = $"/api/{WebApiConstans.CURRENT_API_VERSION}";

    public CompleteRepairFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CompleteRepair_Should_SetRepairStatusAndFinalCost_WhenRepairIsInProgress()
    {
        // Arrange
        var scenario = await RepairScenarioHelper.CreateScheduledRepairAsync(_client, _factory);

        using (var setupScope = _factory.Services.CreateScope())
        {
            var setupDbContext = setupScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var repair = await setupDbContext.Repairs.SingleAsync(r => r.Id == scenario.RepairId);
            var startResult = repair.StartRepair();
            startResult.IsSuccess.Should().BeTrue();
            await setupDbContext.SaveChangesAsync();
        }

        _client.SetBearerToken(scenario.WorkshopToken);
        var completeUrl = $"{BASE_API_URL}{RepairPrefix.COMPLETE.Replace("{id}", scenario.RepairId.ToString())}";
        var request = new CompleteRepairRequest(2250m, "PLN");

        // Act
        var response = await _client.PutAsJsonAsync(completeUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var assertScope = _factory.Services.CreateScope();
        var assertDbContext = assertScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var completedRepair = await assertDbContext.Repairs.SingleAsync(r => r.Id == scenario.RepairId);
        completedRepair.Status.Should().Be(ERepairStatus.Completed);
        completedRepair.FinalCost.Should().NotBeNull();
        completedRepair.FinalCost!.Amount.Should().Be(2250m);
        completedRepair.FinalCost.Currency.Should().Be("PLN");
    }
}



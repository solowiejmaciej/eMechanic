namespace eMechanic.Integration.Tests.Repair;

using System.Net;
using API.Constans;
using API.Features.Repair;
using Domain.Repair.Enums;
using FluentAssertions;
using Helpers;
using Infrastructure.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestContainers;

[Collection("Sequential")]
public class StartRepairFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly HttpClient _client;
    private const string BASE_API_URL = $"/api/{WebApiConstans.CURRENT_API_VERSION}";

    public StartRepairFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task StartRepair_Should_SetRepairStatusToInProgress_WhenWorkshopIsAssigned()
    {
        // Arrange
        var scenario = await RepairScenarioHelper.CreateScheduledRepairAsync(_client, _factory);
        _client.SetBearerToken(scenario.WorkshopToken);
        var startUrl = $"{BASE_API_URL}{RepairPrefix.START.Replace("{id}", scenario.RepairId.ToString())}";

        // Act
        var response = await _client.PutAsync(startUrl, null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var scope = _factory.Services.CreateScope();
        var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var repair = await appDbContext.Repairs.SingleAsync(r => r.Id == scenario.RepairId);
        repair.Status.Should().Be(ERepairStatus.InProgress);
    }
}



namespace eMechanic.Integration.Tests.Repair;

using System.Net;
using System.Net.Http.Json;
using API.Constans;
using API.Features.Repair;
using Application.Repair.Features.Get.ById;
using FluentAssertions;
using Helpers;
using TestContainers;

[Collection("Sequential")]
public class GetRepairByIdFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly HttpClient _client;
    private const string BASE_API_URL = $"/api/{WebApiConstans.CURRENT_API_VERSION}";

    public GetRepairByIdFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetRepairById_Should_ReturnRepairDetailsJson_WhenRequesterIsAssignedWorkshop()
    {
        // Arrange
        var scenario = await RepairScenarioHelper.CreateScheduledRepairAsync(_client, _factory);
        _client.SetBearerToken(scenario.WorkshopToken);
        var getUrl = $"{BASE_API_URL}{RepairPrefix.GET_BY_ID.Replace("{id}", scenario.RepairId.ToString())}";

        // Act
        var response = await _client.GetAsync(getUrl);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<RepairResponse>();
        content.Should().NotBeNull();
        content.Id.Should().Be(scenario.RepairId);
        content.RepairRequestId.Should().Be(scenario.RepairRequestId);
        content.VehicleId.Should().Be(scenario.VehicleId);
        content.Status.Should().Be("Scheduled");
        content.EstimatedCost.Amount.Should().Be(2000m);
        content.EstimatedCost.Currency.Should().Be("PLN");
        content.FinalCost.Should().BeNull();
    }
}



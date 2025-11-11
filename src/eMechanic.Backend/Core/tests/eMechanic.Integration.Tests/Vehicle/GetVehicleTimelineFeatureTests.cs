namespace eMechanic.Integration.Tests.Vehicle;

using System.Net;
using System.Net.Http.Json;
using API.Features.Vehicle.Create.Request;
using API.Features.Vehicle.Update.Request;
using Application.Vehicle.Features.Get;
using Common.Result;
using Domain.Vehicle;
using Domain.Vehicle.Enums;
using FluentAssertions;
using Helpers;
using TestContainers;

public class GetVehicleTimelineFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly AuthHelper _authHelper;

    public GetVehicleTimelineFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _authHelper = new AuthHelper(_client);
    }

    private async Task<(Guid VehicleId, string Token, CreateVehicleRequest CreateRequest)> CreateVehicleForTestUser()
    {
        var authResponse = await _authHelper.CreateAndLoginUserAsync($"user-timeline-{Guid.NewGuid()}@int.com");
        _client.SetBearerToken(authResponse.Token);

        var createRequest = new CreateVehicleRequest(
            $"V1N{Guid.NewGuid().ToString("N")[..14]}",
            "Timeline Manufacturer",
            "Timeline Model",
            "2021",
            1.4m,
            20000,
            EMileageUnit.Kilometers,
            "PZ1W924",
            124,
            EFuelType.Gasoline,
            EBodyType.Hatchback,
            EVehicleType.Passenger);

        var createResponse = await _client.PostAsJsonAsync("/api/v1/vehicles", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var createdContent = await createResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var vehicleId = createdContent!["vehicleId"];

        return (vehicleId, authResponse.Token, createRequest);
    }

    [Fact]
    public async Task GetVehicleTimeline_Should_ReturnCorrectEvents_AfterCreationAndUpdate()
    {
        // Arrange
        var (vehicleId, token, createRequest) = await CreateVehicleForTestUser();
        _client.SetBearerToken(token);

        // Act 1
        var responseAfterCreate = await _client.GetAsync($"/api/v1/vehicles/{vehicleId}/timeline?pageNumber=1&pageSize=10");

        // Assert 1
        responseAfterCreate.StatusCode.Should().Be(HttpStatusCode.OK);
        var timelineAfterCreate = await responseAfterCreate.Content.ReadFromJsonAsync<PaginationResult<VehicleTimelineResponse>>();

        timelineAfterCreate.Should().NotBeNull();
        timelineAfterCreate!.TotalCount.Should().Be(1);
        timelineAfterCreate.Items.Count().Should().Be(1);
        var createEvent = timelineAfterCreate.Items.First();
        createEvent.EventType.Should().Be(nameof(VehicleCreatedDomainEvent));
        createEvent.Data.Should().Contain(createRequest.Vin.ToUpperInvariant());
        createEvent.Data.Should().Contain(createRequest.Manufacturer);
        createEvent.Data.Should().Contain(createRequest.Mileage.ToString(System.Globalization.CultureInfo.InvariantCulture));

        // Arrange 2
        var updateRequest = new UpdateVehicleRequest(
            createRequest.Vin,
            "UPDATED Manufacturer",
            createRequest.Model,
            createRequest.ProductionYear,
            2.0m,
            25000,
            EMileageUnit.Kilometers,
            "PZ1W924",
            124,
            EFuelType.LPG,
            createRequest.BodyType,
            createRequest.VehicleType
        );

        // Act 2:
        var updateResponse = await _client.PutAsJsonAsync($"/api/v1/vehicles/{vehicleId}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Act 3:
        var responseAfterUpdate = await _client.GetAsync($"/api/v1/vehicles/{vehicleId}/timeline?pageNumber=1&pageSize=10");

        // Assert 3:
        responseAfterUpdate.StatusCode.Should().Be(HttpStatusCode.OK);
        var timelineAfterUpdate = await responseAfterUpdate.Content.ReadFromJsonAsync<PaginationResult<VehicleTimelineResponse>>();

        timelineAfterUpdate.Should().NotBeNull();
        timelineAfterUpdate!.TotalCount.Should().Be(5);
        timelineAfterUpdate.Items.Count().Should().Be(5);

        var events = timelineAfterUpdate.Items.OrderBy(t => t.CreatedAt).ToList();
        events[0].EventType.Should().Be("VehicleCreatedDomainEvent");

        var eventTypes = events.Select(e => e.EventType).ToList();
        eventTypes.Should().Contain(nameof(VehicleManufacturerChangedDomainEvent));
        eventTypes.Should().Contain(nameof(VehicleEngineCapacityChangedDomainEvent));
        eventTypes.Should().Contain(nameof(VehicleFuelTypeChangedDomainEvent));
        eventTypes.Should().Contain(nameof(VehicleFuelTypeChangedDomainEvent));

        var manufacturerEvent = events.First(e => e.EventType == nameof(VehicleManufacturerChangedDomainEvent));
        manufacturerEvent.Data.Should().Contain(createRequest.Manufacturer);
        manufacturerEvent.Data.Should().Contain(updateRequest.Manufacturer);

        var mileageEvent = events.First(e => e.EventType == nameof(VehicleMileageChangedDomainEvent));
        mileageEvent.Data.Should().Contain(createRequest.Mileage.ToString(System.Globalization.CultureInfo.InvariantCulture));
        mileageEvent.Data.Should().Contain(updateRequest.MileageValue.ToString(System.Globalization.CultureInfo.InvariantCulture));

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task GetVehicleTimeline_Should_HandlePaginationCorrectly()
    {
        // Arrange
        var (vehicleId, token, createRequest) = await CreateVehicleForTestUser();
        _client.SetBearerToken(token);

        var updateRequest1 = new UpdateVehicleRequest(createRequest.Vin, "Update 1", createRequest.Model, createRequest.ProductionYear, createRequest.EngineCapacity, 21000, createRequest.MileageUnit,"PZ1W924", 124,createRequest.FuelType, createRequest.BodyType, createRequest.VehicleType);
        var updateRequest2 = new UpdateVehicleRequest(createRequest.Vin, "Update 1", "Update 2", createRequest.ProductionYear, createRequest.EngineCapacity, 22000, createRequest.MileageUnit, "PZ1W924", 124,createRequest.FuelType, createRequest.BodyType, createRequest.VehicleType);
        var updateRequest3 = new UpdateVehicleRequest(createRequest.Vin, "Update 1", "Update 2", createRequest.ProductionYear, createRequest.EngineCapacity, 23000, createRequest.MileageUnit, "PZ1W924", 124,EFuelType.Diesel, createRequest.BodyType, createRequest.VehicleType);

        await _client.PutAsJsonAsync($"/api/v1/vehicles/{vehicleId}", updateRequest1);
        await _client.PutAsJsonAsync($"/api/v1/vehicles/{vehicleId}", updateRequest2);
        await _client.PutAsJsonAsync($"/api/v1/vehicles/{vehicleId}", updateRequest3);

        // Act
        var response = await _client.GetAsync($"/api/v1/vehicles/{vehicleId}/timeline?pageNumber=2&pageSize=3");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginationResult<VehicleTimelineResponse>>();

        result.Should().NotBeNull();
        result!.TotalCount.Should().Be(7);
        result.TotalPages.Should().Be(3);
        result.PageNumber.Should().Be(2);
        result.Items.Count().Should().Be(3);

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task GetVehicleTimeline_Should_ReturnNotFound_WhenVehicleBelongsToAnotherUser()
    {
        // Arrange
        var (vehicleId, _, _) = await CreateVehicleForTestUser();
        _client.ClearBearerToken();

        var authResponse = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(authResponse.Token);

        // Act
        var response = await _client.GetAsync($"/api/v1/vehicles/{vehicleId}/timeline?pageNumber=1&pageSize=3");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task GetVehicleTimeline_Should_ReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var (vehicleId, _, _) = await CreateVehicleForTestUser();
        _client.ClearBearerToken();

        // Act
        var response = await _client.GetAsync($"/api/v1/vehicles/{vehicleId}/timeline");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetVehicleTimeline_Should_ReturnForbidden_WhenWorkshopTokenIsUsed()
    {
        // Arrange
        var (vehicleId, _, _) = await CreateVehicleForTestUser();
        _client.ClearBearerToken();

        var authResponse = await _authHelper.CreateAndLoginWorkshopAsync();
        _client.SetBearerToken(authResponse.Token);

        // Act
        var response = await _client.GetAsync($"/api/v1/vehicles/{vehicleId}/timeline");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        _client.ClearBearerToken();
    }
}

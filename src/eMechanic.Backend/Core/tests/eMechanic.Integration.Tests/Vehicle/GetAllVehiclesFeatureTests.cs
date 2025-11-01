namespace eMechanic.Integration.Tests.Vehicle;

using System.Net;
using System.Net.Http.Json;
using API.Features.Vehicle.Create.Request;
using Application.Vehicle.Get;
using Common.Result;
using Domain.Vehicle.Enums;
using FluentAssertions;
using Helpers;
using TestContainers;

public class GetAllVehiclesFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly AuthHelper _authHelper;

    public GetAllVehiclesFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _authHelper = new AuthHelper(_client);
    }

    private async Task<Guid> CreateVehicleForUser(string manufacturer, string token)
    {
        _client.SetBearerToken(token);
        var createRequest = new CreateVehicleRequest(
            $"V1N{Guid.NewGuid().ToString("N")[..14]}", manufacturer, "GetAll Model", "2021",
            1.4m, EFuelType.Gasoline, EBodyType.Hatchback, EVehicleType.Passenger);

        var createResponse = await _client.PostAsJsonAsync("/api/v1/vehicles", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var createdContent = await createResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        return createdContent!["vehicleId"];
    }

    [Fact]
    public async Task GetAllVehicles_Should_ReturnOkAndPaginatedList_WhenUserIsAuthenticated()
    {
        // Arrange
        var (_, tokenA) = await _authHelper.CreateAndLoginUserAsync("userA-getall@int.com");
        await CreateVehicleForUser("ManufacturerA1", tokenA);
        await CreateVehicleForUser("ManufacturerA2", tokenA);

        var (_, tokenB) = await _authHelper.CreateAndLoginUserAsync("userB-getall@int.com");
        await CreateVehicleForUser("ManufacturerB1", tokenB);

        _client.SetBearerToken(tokenA);

        // Act
        var response = await _client.GetAsync("/api/v1/vehicles?pageNumber=1&pageSize=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginationResult<VehicleResponse>>();

        result.Should().NotBeNull();
        result!.TotalCount.Should().Be(2);
        result.Items.Count().Should().Be(2);
        result.Items.Should().Contain(v => v.Manufacturer == "ManufacturerA1");
        result.Items.Should().Contain(v => v.Manufacturer == "ManufacturerA2");
        result.Items.Should().NotContain(v => v.Manufacturer == "ManufacturerB1");

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task GetAllVehicles_Should_HandlePaginationCorrectly()
    {
        // Arrange
        var (_, token) = await _authHelper.CreateAndLoginUserAsync("userC-getall@int.com");
        await CreateVehicleForUser("CarA", token);
        await CreateVehicleForUser("CarB", token);
        await CreateVehicleForUser("CarC", token);

        _client.SetBearerToken(token);

        // Act
        var response = await _client.GetAsync("/api/v1/vehicles?pageNumber=2&pageSize=2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginationResult<VehicleResponse>>();

        result.Should().NotBeNull();
        result!.TotalCount.Should().Be(3);
        result.TotalPages.Should().Be(2);
        result.PageNumber.Should().Be(2);
        result.Items.Count().Should().Be(1);

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task GetAllVehicles_Should_ReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _client.ClearBearerToken();

        // Act
        var response = await _client.GetAsync("/api/v1/vehicles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllVehicles_Should_ReturnForbiddenOrUnauthorized_WhenWorkshopTokenIsUsed()
    {
        // Arrange
        var (_, workshopToken) = await _authHelper.CreateAndLoginWorkshopAsync();
        _client.SetBearerToken(workshopToken);

        // Act
        var response = await _client.GetAsync("/api/v1/vehicles?pageNumber=2&pageSize=2");
        var test =response.Content;

        // Assert
        response.StatusCode.Should().Match(code =>
            code == HttpStatusCode.Unauthorized || code == HttpStatusCode.Forbidden);

        _client.ClearBearerToken();
    }
}

namespace eMechanic.Integration.Tests.Vehicle;

using System.Net;
using System.Net.Http.Json;
using API.Constans;
using API.Features.Vehicle;
using API.Features.Vehicle.Vehicle.Create.Request;
using Application.Vehicle.Features.Get;
using Common.Result;
using Domain.Vehicle.Enums;
using FluentAssertions;
using Helpers;
using TestContainers;

public class GetAllVehiclesFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly AuthHelper _authHelper;
    private const string BASE_API_URL = $"/api/{WebApiConstans.CURRENT_API_VERSION}{VehiclePrefix.GET_ALL}";

    public GetAllVehiclesFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _authHelper = new AuthHelper(_client);
    }

    private async Task<Guid> CreateVehicleForUser(string manufacturer, string token)
    {
        _client.SetBearerToken(token);
        var createRequest = new CreateVehicleRequest(
            $"V1N{Guid.NewGuid().ToString("N")[..14]}",
            manufacturer,
            "GetAll Model",
            "2021",
            1.4m,
            200,
            EMileageUnit.Miles,
            "PZ1W924",
            124,
            EFuelType.Gasoline,
            EBodyType.Hatchback,
            EVehicleType.Passenger);

        var createResponse = await _client.PostAsJsonAsync(BASE_API_URL, createRequest);
        createResponse.EnsureSuccessStatusCode();
        var createdContent = await createResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        return createdContent!["vehicleId"];
    }

    [Fact]
    public async Task GetAllVehicles_Should_ReturnOkAndPaginatedList_WhenUserIsAuthenticated()
    {
        // Arrange
        var authResponseA = await _authHelper.CreateAndLoginUserAsync("userA-getall@int.com");
        await CreateVehicleForUser("ManufacturerA1", authResponseA.Token);
        await CreateVehicleForUser("ManufacturerA2", authResponseA.Token);

        var authResponseB = await _authHelper.CreateAndLoginUserAsync("userB-getall@int.com");
        await CreateVehicleForUser("ManufacturerB1", authResponseB.Token);

        _client.SetBearerToken(authResponseA.Token);

        // Act
        var response = await _client.GetAsync($"{BASE_API_URL}?pageNumber=1&pageSize=5");

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
        var authResponse = await _authHelper.CreateAndLoginUserAsync("userC-getall@int.com");
        await CreateVehicleForUser("CarA", authResponse.Token);
        await CreateVehicleForUser("CarB", authResponse.Token);
        await CreateVehicleForUser("CarC", authResponse.Token);

        _client.SetBearerToken(authResponse.Token);

        // Act
        var response = await _client.GetAsync($"{BASE_API_URL}?pageNumber=2&pageSize=2");

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
        var response = await _client.GetAsync(BASE_API_URL);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllVehicles_Should_ReturnForbiddenOrUnauthorized_WhenWorkshopTokenIsUsed()
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginWorkshopAsync();
        _client.SetBearerToken(authResponse.Token);

        // Act
        var response = await _client.GetAsync($"{BASE_API_URL}?pageNumber=2&pageSize=2");

        // Assert
        response.StatusCode.Should().Match(code =>
            code == HttpStatusCode.Unauthorized || code == HttpStatusCode.Forbidden);

        _client.ClearBearerToken();
    }
}

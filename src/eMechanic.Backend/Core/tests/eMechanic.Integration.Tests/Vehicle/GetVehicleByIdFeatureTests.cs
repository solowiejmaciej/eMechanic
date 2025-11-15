namespace eMechanic.Integration.Tests.Vehicle;

using System.Net;
using System.Net.Http.Json;
using API.Constans;
using API.Features.Vehicle;
using API.Features.Vehicle.Vehicle.Create.Request;
using Application.Vehicle.Features.Get;
using Domain.Vehicle.Enums;
using FluentAssertions;
using Helpers;
using TestContainers;


public class GetVehicleByIdFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly AuthHelper _authHelper;
    private const string BASE_API_URL = $"/api/{WebApiConstans.CURRENT_API_VERSION}{VehiclePrefix.GET_BY_ID}";
    private const string CREATE_BASE_API_URL = $"/api/{WebApiConstans.CURRENT_API_VERSION}{VehiclePrefix.CREATE}";

    public GetVehicleByIdFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _authHelper = new AuthHelper(_client);
    }

    private async Task<(Guid UserId, Guid VehicleId, string Token)> CreateVehicleForTestUser()
    {
        var authResponse = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(authResponse.Token);

        var createRequest = new CreateVehicleRequest(
            $"V1N{Guid.NewGuid().ToString("N")[..14]}",
            "GetById Manufacturer",
            "GetById Model",
            "2021",
            1.4m,
            200,
            EMileageUnit.Kilometers,
            "PZ1W924",
            124,
            EFuelType.Gasoline,
            EBodyType.Hatchback,
            EVehicleType.Passenger);
        var createResponse = await _client.PostAsJsonAsync(CREATE_BASE_API_URL, createRequest);
        createResponse.EnsureSuccessStatusCode();
        var createdContent = await createResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var vehicleId = createdContent!["vehicleId"];

        return (authResponse.DomainId, vehicleId, authResponse.Token);
    }

    [Fact]
    public async Task GetVehicleById_Should_ReturnOkAndVehicle_WhenVehicleExistsForUser()
    {
        // Arrange
        var (_, vehicleId, token) = await CreateVehicleForTestUser();
        _client.SetBearerToken(token);

        // Act
        var response = await _client.GetAsync($"{BASE_API_URL.Replace("{id:guid}", vehicleId.ToString())}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var vehicleResponse = await response.Content.ReadFromJsonAsync<VehicleResponse>();
        vehicleResponse.Should().NotBeNull();
        vehicleResponse!.Id.Should().Be(vehicleId);
        vehicleResponse.Manufacturer.Should().Be("GetById Manufacturer");

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task GetVehicleById_Should_ReturnNotFound_WhenVehicleDoesNotExist()
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(authResponse.Token);
        var nonExistentVehicleId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"{BASE_API_URL.Replace("{id:guid}", nonExistentVehicleId.ToString())}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task GetVehicleById_Should_ReturnNotFound_WhenVehicleExistsButBelongsToAnotherUser()
    {
        // Arrange
        var (_, vehicleId, _) = await CreateVehicleForTestUser();
        _client.ClearBearerToken();

        var authResponse = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(authResponse.Token);

        // Act
        var response = await _client.GetAsync($"{BASE_API_URL.Replace("{id:guid}", vehicleId.ToString())}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task GetVehicleById_Should_ReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var (_, vehicleId, _) = await CreateVehicleForTestUser();
        _client.ClearBearerToken();

        // Act
        var response = await _client.GetAsync($"{BASE_API_URL.Replace("{id:guid}", vehicleId.ToString())}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetVehicleById_Should_ReturnForbiddenOrUnauthorized_WhenWorkshopTokenIsUsed()
    {
        // Arrange
        var (_, vehicleId, _) = await CreateVehicleForTestUser();
        _client.ClearBearerToken();

        var authResponse = await _authHelper.CreateAndLoginWorkshopAsync();
        _client.SetBearerToken(authResponse.Token);

        // Act
        var response = await _client.GetAsync($"{BASE_API_URL.Replace("{id:guid}", vehicleId.ToString())}");

        // Assert
        response.StatusCode.Should().Match(code =>
            code == HttpStatusCode.Unauthorized || code == HttpStatusCode.Forbidden || code == HttpStatusCode.NotFound,
             "Workshop token should not allow getting user's vehicle");

        _client.ClearBearerToken();
    }
}

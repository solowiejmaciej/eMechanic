namespace eMechanic.Integration.Tests.Vehicle;

using System.Net;
using System.Net.Http.Json;
using API.Features.Vehicle.Vehicle.Create.Request;
using Domain.Vehicle.Enums;
using eMechanic.API.Constans;
using eMechanic.API.Features.Vehicle;
using FluentAssertions;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using TestContainers;

public class CreateVehicleFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly AuthHelper _authHelper;
    private const string BASE_API_URL = $"/api/{WebApiConstans.CURRENT_API_VERSION}{VehiclePrefix.CREATE}";

    public CreateVehicleFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _authHelper = new AuthHelper(_client);
    }

    private CreateVehicleRequest CreateValidRequest() => new(
       $"V1N{Guid.NewGuid().ToString("N")[..14]}",
       "Integration Test Manufacturer",
       "Integration Test Model",
       "2024",
       1.8m,
       200,
       EMileageUnit.Miles,
       "PZ1W924",
       124,
       EFuelType.Hybrid,
       EBodyType.SUV,
       EVehicleType.Passenger
   );

    [Fact]
    public async Task CreateVehicle_Should_ReturnCreated_WhenDataIsValidAndUserIsAuthenticated()
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(authResponse.Token);
        var request = CreateValidRequest();

        // Act
        var response = await _client.PostAsJsonAsync(BASE_API_URL, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        var createdResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        createdResponse.Should().NotBeNull();
        createdResponse!["vehicleId"].Should().NotBeEmpty();

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task CreateVehicle_Should_ReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var request = CreateValidRequest();
        _client.ClearBearerToken();

        // Act
        var response = await _client.PostAsJsonAsync(BASE_API_URL, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateVehicle_Should_ReturnForbidden_WhenWorkshopTokenIsUsed()
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginWorkshopAsync();
        _client.SetBearerToken(authResponse.Token);
        var request = CreateValidRequest();

        // Act
        var response = await _client.PostAsJsonAsync(BASE_API_URL, request);

        // Assert
        response.StatusCode.Should().Match(code =>
            code == HttpStatusCode.Unauthorized || code == HttpStatusCode.Forbidden || code == HttpStatusCode.BadRequest,
             "Workshop token should not allow vehicle creation");


        _client.ClearBearerToken();
    }


    [Theory]
    [InlineData("")]
    [InlineData("INVALID_VIN")]
    public async Task CreateVehicle_Should_ReturnBadRequest_WhenVinIsInvalid(string invalidVin)
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(authResponse.Token);
        var request = CreateValidRequest() with { Vin = invalidVin };

        // Act
        var response = await _client.PostAsJsonAsync(BASE_API_URL, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().ContainKey("Vin");

        _client.ClearBearerToken();
    }


    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateVehicle_Should_ReturnBadRequest_WhenManufacturerIsEmpty(string? invalidManufacturer)
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(authResponse.Token);
        var request = CreateValidRequest() with { Manufacturer = invalidManufacturer! };

        // Act
        var response = await _client.PostAsJsonAsync(BASE_API_URL, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("Manufacturer");

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task CreateVehicle_Should_ReturnBadRequest_WhenManufacturerIsTooLong()
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(authResponse.Token);
        var longManufacturer = new string('a', 101);
        var request = CreateValidRequest() with { Manufacturer = longManufacturer };

        // Act
        var response = await _client.PostAsJsonAsync(BASE_API_URL, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("Manufacturer");

        _client.ClearBearerToken();
    }


    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateVehicle_Should_ReturnBadRequest_WhenModelIsEmpty(string? invalidModel)
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(authResponse.Token);
        var request = CreateValidRequest() with { Model = invalidModel! };

        // Act
        var response = await _client.PostAsJsonAsync(BASE_API_URL, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("Model");

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task CreateVehicle_Should_ReturnBadRequest_WhenModelIsTooLong()
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(authResponse.Token);
        var longModel = new string('b', 101);
        var request = CreateValidRequest() with { Model = longModel };

        // Act
        var response = await _client.PostAsJsonAsync(BASE_API_URL, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("Model");

        _client.ClearBearerToken();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("202")]
    [InlineData("20234")]
    [InlineData("abcd")]
    public async Task CreateVehicle_Should_ReturnBadRequest_WhenProductionYearIsInvalid(string? invalidYear)
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(authResponse.Token);
        var request = CreateValidRequest() with { ProductionYear = invalidYear! };

        // Act
        var response = await _client.PostAsJsonAsync(BASE_API_URL, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        _client.ClearBearerToken();
    }


    [Theory]
    [InlineData(0)]
    [InlineData(-1.5)]
    public async Task CreateVehicle_Should_ReturnBadRequest_WhenEngineCapacityIsInvalid(decimal invalidCapacity)
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(authResponse.Token);
        var request = CreateValidRequest() with { EngineCapacity = invalidCapacity };

        // Act
        var response = await _client.PostAsJsonAsync(BASE_API_URL, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("EngineCapacity");

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task CreateVehicle_Should_ReturnBadRequest_WhenFuelTypeIsNone()
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(authResponse.Token);
        var request = CreateValidRequest() with { FuelType = EFuelType.None };

        // Act
        var response = await _client.PostAsJsonAsync(BASE_API_URL, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("FuelType");

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task CreateVehicle_Should_ReturnBadRequest_WhenBodyTypeIsNone()
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(authResponse.Token);
        var request = CreateValidRequest() with { BodyType = EBodyType.None };

        // Act
        var response = await _client.PostAsJsonAsync(BASE_API_URL, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("BodyType");

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task CreateVehicle_Should_ReturnCreated_WhenVehicleTypeIsMotorcycleAndBodyTypeIsNone()
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(authResponse.Token);

        var request = CreateValidRequest() with
        {
            VehicleType = EVehicleType.Motorcycle,
            BodyType = EBodyType.None
        };

        // Act
        var response = await _client.PostAsJsonAsync(BASE_API_URL, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        _client.ClearBearerToken();
    }

    [Theory]
    [InlineData("INVALID PLATE !@#")]
    [InlineData("TOOOOOOOOOOOOOLONGPLATE")]
    public async Task CreateVehicle_Should_ReturnBadRequest_WhenLicensePlateIsInvalid(string invalidPlate)
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(authResponse.Token);
        var request = CreateValidRequest() with { LicensePlate = invalidPlate };

        // Act
        var response = await _client.PostAsJsonAsync(BASE_API_URL, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("LicensePlate");

        _client.ClearBearerToken();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    [InlineData(99999)]
    public async Task CreateVehicle_Should_ReturnBadRequest_WhenHorsePowerIsInvalid(int invalidHp)
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(authResponse.Token);
        var request = CreateValidRequest() with { HorsePower = invalidHp };

        // Act
        var response = await _client.PostAsJsonAsync(BASE_API_URL, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("HorsePower");

        _client.ClearBearerToken();
    }
}

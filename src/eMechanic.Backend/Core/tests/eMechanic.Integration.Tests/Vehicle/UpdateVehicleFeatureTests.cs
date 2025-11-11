namespace eMechanic.Integration.Tests.Vehicle;

using System.Net;
using System.Net.Http.Json;
using API.Features.Vehicle.Create.Request;
using API.Features.Vehicle.Update.Request;
using Application.Vehicle.Features.Get;
using Domain.Vehicle.Enums;
using FluentAssertions;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using TestContainers;

public class UpdateVehicleFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly AuthHelper _authHelper;

    public UpdateVehicleFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _authHelper = new AuthHelper(_client);
    }

    private async Task<(Guid UserId, Guid VehicleId, string Token)> CreateVehicleForTestUser(string suffix = "update")
    {
        var authResponse = await _authHelper.CreateAndLoginUserAsync($"user-{suffix}-{Guid.NewGuid()}@int.com");
        _client.SetBearerToken(authResponse.Token);

        var createRequest = new CreateVehicleRequest(
            $"V1N{Guid.NewGuid().ToString("N")[..14]}",
            $"UpdateTest Manufacturer {suffix}",
            $"UpdateTest Model {suffix}",
            "2020",
            1.9m,
            200,
            EMileageUnit.Miles,
            "PZ1W924",
            124,
            EFuelType.Diesel,
            EBodyType.Sedan,
            EVehicleType.Passenger);
        var createResponse = await _client.PostAsJsonAsync("/api/v1/vehicles", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var createdContent = await createResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
        var vehicleId = createdContent!["vehicleId"];

        return (authResponse.DomainId, vehicleId, authResponse.Token);
    }

    private UpdateVehicleRequest CreateValidUpdateRequest() => new(
        $"V1N{Guid.NewGuid().ToString("N")[..14]}",
        "Updated Manufacturer",
        "Updated Model",
        "2022",
        2.5m,
        200,
        EMileageUnit.Kilometers,
        "PZ1W924",
        124,
        EFuelType.Electric,
        EBodyType.Coupe,
        EVehicleType.Passenger
    );

    [Fact]
    public async Task UpdateVehicle_Should_ReturnNoContent_WhenDataIsValidAndUserIsAuthenticated()
    {
        // Arrange
        var (_, vehicleId, token) = await CreateVehicleForTestUser();
        _client.SetBearerToken(token);
        var updateRequest = CreateValidUpdateRequest();

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/vehicles/{vehicleId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/v1/vehicles/{vehicleId}");
        getResponse.EnsureSuccessStatusCode();
        var updatedVehicle = await getResponse.Content.ReadFromJsonAsync<VehicleResponse>();
        updatedVehicle!.Manufacturer.Should().Be("Updated Manufacturer");
        updatedVehicle!.Model.Should().Be("Updated Model");
        updatedVehicle!.Vin.Should().Be(updateRequest.Vin.ToUpperInvariant());
        updatedVehicle!.LicensePlate.Should().Be("PZ1W924");
        updatedVehicle!.HorsePower.Should().Be(124);

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task UpdateVehicle_Should_ReturnNotFound_WhenVehicleDoesNotExist()
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(authResponse.Token);
        var nonExistentVehicleId = Guid.NewGuid();
        var updateRequest = CreateValidUpdateRequest();

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/vehicles/{nonExistentVehicleId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task UpdateVehicle_Should_ReturnNotFound_WhenVehicleBelongsToAnotherUser()
    {
        // Arrange
        var (_, vehicleId, _) = await CreateVehicleForTestUser("owner1");
        _client.ClearBearerToken();

        var authResponse = await _authHelper.CreateAndLoginUserAsync("owner2@gmail.com");
        _client.SetBearerToken(authResponse.Token);
        var updateRequest = CreateValidUpdateRequest();

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/vehicles/{vehicleId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task UpdateVehicle_Should_ReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var (_, vehicleId, _) = await CreateVehicleForTestUser();
        _client.ClearBearerToken();
        var updateRequest = CreateValidUpdateRequest();

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/vehicles/{vehicleId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateVehicle_Should_ReturnForbiddenOrUnauthorized_WhenWorkshopTokenIsUsed()
    {
        // Arrange
        var (_, vehicleId, _) = await CreateVehicleForTestUser();
        _client.ClearBearerToken();

        var authResponse = await _authHelper.CreateAndLoginWorkshopAsync();
        _client.SetBearerToken(authResponse.Token);
        var updateRequest = CreateValidUpdateRequest();


        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/vehicles/{vehicleId}", updateRequest);

        // Assert
        response.StatusCode.Should().Match(code =>
            code == HttpStatusCode.Unauthorized || code == HttpStatusCode.Forbidden,
             "Workshop token should not allow updating user's vehicle");

        _client.ClearBearerToken();
    }

    [Theory]
    [InlineData("")]
    [InlineData("INVALID_VIN_LEN")]
    public async Task UpdateVehicle_Should_ReturnBadRequest_WhenVinIsInvalid(string invalidVin)
    {
        // Arrange
        var (_, vehicleId, token) = await CreateVehicleForTestUser();
        _client.SetBearerToken(token);
        var updateRequest = CreateValidUpdateRequest() with { Vin = invalidVin };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/vehicles/{vehicleId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("Vin");

        _client.ClearBearerToken();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task UpdateVehicle_Should_ReturnBadRequest_WhenManufacturerIsEmpty(string? invalidManufacturer)
    {
        // Arrange
        var (_, vehicleId, token) = await CreateVehicleForTestUser();
        _client.SetBearerToken(token);
        var updateRequest = CreateValidUpdateRequest() with { Manufacturer = invalidManufacturer! };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/vehicles/{vehicleId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("Manufacturer");

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task UpdateVehicle_Should_ReturnBadRequest_WhenManufacturerIsTooLong()
    {
        // Arrange
        var (_, vehicleId, token) = await CreateVehicleForTestUser();
        _client.SetBearerToken(token);
        var longManufacturer = new string('a', 101);
        var updateRequest = CreateValidUpdateRequest() with { Manufacturer = longManufacturer };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/vehicles/{vehicleId}", updateRequest);

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
    public async Task UpdateVehicle_Should_ReturnBadRequest_WhenModelIsEmpty(string? invalidModel)
    {
        // Arrange
        var (_, vehicleId, token) = await CreateVehicleForTestUser();
        _client.SetBearerToken(token);
        var updateRequest = CreateValidUpdateRequest() with { Model = invalidModel! };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/vehicles/{vehicleId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("Model");

        _client.ClearBearerToken();
    }

     [Fact]
    public async Task UpdateVehicle_Should_ReturnBadRequest_WhenModelIsTooLong()
    {
        // Arrange
        var (_, vehicleId, token) = await CreateVehicleForTestUser();
        _client.SetBearerToken(token);
        var longModel = new string('b', 101);
        var updateRequest = CreateValidUpdateRequest() with { Model = longModel };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/vehicles/{vehicleId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("Model");

        _client.ClearBearerToken();
    }


    [Theory]
    [InlineData("")]
    [InlineData("202")]
    [InlineData("20234")]
    [InlineData("abcd")]
    public async Task UpdateVehicle_Should_ReturnBadRequest_WhenProductionYearIsInvalid(string invalidYear)
    {
        // Arrange
        var (_, vehicleId, token) = await CreateVehicleForTestUser();
        _client.SetBearerToken(token);
        var updateRequest = CreateValidUpdateRequest() with { ProductionYear = invalidYear };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/vehicles/{vehicleId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        _client.ClearBearerToken();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-2.0)]
    public async Task UpdateVehicle_Should_ReturnBadRequest_WhenEngineCapacityIsInvalid(decimal invalidCapacity)
    {
        // Arrange
        var (_, vehicleId, token) = await CreateVehicleForTestUser();
        _client.SetBearerToken(token);
        var updateRequest = CreateValidUpdateRequest() with { EngineCapacity = invalidCapacity };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/vehicles/{vehicleId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("EngineCapacity");

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task UpdateVehicle_Should_ReturnBadRequest_WhenFuelTypeIsNone()
    {
        // Arrange
        var (_, vehicleId, token) = await CreateVehicleForTestUser();
        _client.SetBearerToken(token);
        var updateRequest = CreateValidUpdateRequest() with { FuelType = EFuelType.None };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/vehicles/{vehicleId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("FuelType");

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task UpdateVehicle_Should_ReturnBadRequest_WhenBodyTypeIsNone()
    {
        // Arrange
        var (_, vehicleId, token) = await CreateVehicleForTestUser();
        _client.SetBearerToken(token);
        var updateRequest = CreateValidUpdateRequest() with { BodyType = EBodyType.None };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/vehicles/{vehicleId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("BodyType");

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task UpdateVehicle_Should_ReturnNoContent_WhenUpdatingToMotorcycleAndBodyTypeNone()
    {
        // Arrange
        var (_, vehicleId, token) = await CreateVehicleForTestUser();
        _client.SetBearerToken(token);

        var updateRequest = CreateValidUpdateRequest() with
        {
            VehicleType = EVehicleType.Motorcycle,
            BodyType = EBodyType.None
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/vehicles/{vehicleId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/v1/vehicles/{vehicleId}");
        getResponse.EnsureSuccessStatusCode();
        var updatedVehicle = await getResponse.Content.ReadFromJsonAsync<VehicleResponse>();
        updatedVehicle!.VehicleType.Should().Be(EVehicleType.Motorcycle);
        updatedVehicle!.BodyType.Should().Be(EBodyType.None);

        _client.ClearBearerToken();
    }

    [Theory]
    [InlineData("INVALID PLATE !@#")]
    public async Task UpdateVehicle_Should_ReturnBadRequest_WhenLicensePlateIsInvalid(string invalidPlate)
    {
        // Arrange
        var (_, vehicleId, token) = await CreateVehicleForTestUser();
        _client.SetBearerToken(token);
        var updateRequest = CreateValidUpdateRequest() with { LicensePlate = invalidPlate };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/vehicles/{vehicleId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("LicensePlate");

        _client.ClearBearerToken();
    }

    [Theory]
    [InlineData(-100)]
    public async Task UpdateVehicle_Should_ReturnBadRequest_WhenHorsePowerIsInvalid(int invalidHp)
    {
        // Arrange
        var (_, vehicleId, token) = await CreateVehicleForTestUser();
        _client.SetBearerToken(token);
        var updateRequest = CreateValidUpdateRequest() with { HorsePower = invalidHp };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/vehicles/{vehicleId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("HorsePower");

        _client.ClearBearerToken();
    }
}

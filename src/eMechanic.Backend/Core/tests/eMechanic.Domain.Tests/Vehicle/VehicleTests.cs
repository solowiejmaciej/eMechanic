namespace eMechanic.Domain.Tests.Vehicle;

using Domain.Vehicle;
using Domain.Vehicle.ValueObjects;
using Domain.Vehicle.Enums;
using Domain.Vehicle.DomainEvents;
using Common.Result;
using System;
using System.Linq;
using FluentAssertions;

public class VehicleTests
{
    private readonly Guid _validOwnerId = Guid.NewGuid();
    private const string VALID_VIN = "JMZGG128271672202";
    private const string VALID_MANUFACTURER = "Mazda";
    private const string VALID_MODEL = "6";
    private const string VALID_YEAR = "2006";
    private const decimal VALID_CAPACITY = 1.9m;
    private const EFuelType VALID_FUEL = EFuelType.Gasoline;
    private const EBodyType VALID_BODY = EBodyType.Sedan;
    private const EVehicleType VALID_TYPE = EVehicleType.Passenger;

    private Vehicle CreateValidVehicle()
    {
        var result = Vehicle.Create(
            _validOwnerId,
            VALID_VIN,
            VALID_MANUFACTURER,
            VALID_MODEL,
            VALID_YEAR,
            VALID_CAPACITY,
            VALID_FUEL,
            VALID_BODY,
            VALID_TYPE);
        result.HasError().Should().BeFalse();
        return result.Value!;
    }


    [Fact]
    public void Create_Should_ReturnSuccessAndVehicle_WhenAllDataIsValid()
    {
        // Act
        var result = Vehicle.Create(
            _validOwnerId,
            VALID_VIN,
            VALID_MANUFACTURER,
            VALID_MODEL,
            VALID_YEAR,
            VALID_CAPACITY,
            VALID_FUEL,
            VALID_BODY,
            VALID_TYPE);

        // Assert
        result.HasError().Should().BeFalse();
        var vehicle = result.Value;
        vehicle.Should().NotBeNull();
        vehicle.Id.Should().NotBeEmpty();
        vehicle.OwnerUserId.Should().Be(_validOwnerId);
        vehicle.Vin.Value.Should().Be(VALID_VIN);
        vehicle.Manufacturer.Value.Should().Be(VALID_MANUFACTURER);
        vehicle.Model.Value.Should().Be(VALID_MODEL);
        vehicle.ProductionYear.Value.Should().Be(VALID_YEAR);
        vehicle.EngineCapacity.Should().NotBeNull();
        vehicle.EngineCapacity!.Value.Should().Be(VALID_CAPACITY);
        vehicle.FuelType.Should().Be(VALID_FUEL);
        vehicle.BodyType.Should().Be(VALID_BODY);
        vehicle.VehicleType.Should().Be(VALID_TYPE);

        vehicle.GetDomainEvents().Should().ContainSingle(e => e is VehicleCreatedDomainEvent);
        var domainEvent = (VehicleCreatedDomainEvent)vehicle.GetDomainEvents().First();
        domainEvent.Vehicle.Id.Should().Be(vehicle.Id);
    }

    [Fact]
    public void Create_Should_ReturnSuccess_WhenEngineCapacityIsNull()
    {
        // Act
        var result = Vehicle.Create(
            _validOwnerId,
            VALID_VIN,
            VALID_MANUFACTURER,
            VALID_MODEL,
            VALID_YEAR,
            null,
            VALID_FUEL,
            VALID_BODY,
            VALID_TYPE);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.EngineCapacity.Should().BeNull();
    }

    [Fact]
    public void Create_Should_ReturnError_WhenOwnerIdIsEmpty()
    {
        // Act
        var result = Vehicle.Create(Guid.Empty, VALID_VIN, VALID_MANUFACTURER, VALID_MODEL, VALID_YEAR, VALID_CAPACITY, VALID_FUEL, VALID_BODY, VALID_TYPE);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("OwnerUserId");
    }

    [Theory]
    [InlineData("INVALIDVINLEN")]
    [InlineData("V!N123456789ABCDE")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_Should_ReturnError_WhenVinIsInvalid(string? invalidVin)
    {
        var result = Vehicle.Create(_validOwnerId, invalidVin, VALID_MANUFACTURER, VALID_MODEL, VALID_YEAR, VALID_CAPACITY, VALID_FUEL, VALID_BODY, VALID_TYPE);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("VIN");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("VeryLongManufacturerNameThatExceedsTheMaximumAllowedLengthOfOneHundredCharactersDefinitelyByTheWayWhyAreYouStillReadingThisVeryVeryLongManufacturerNameGoMakeSomeCoffe")]
    public void Create_Should_ReturnError_WhenManufacturerIsInvalid(string? invalidManufacturer)
    {
        // Act
        var result = Vehicle.Create(_validOwnerId, VALID_VIN, invalidManufacturer, VALID_MODEL, VALID_YEAR, VALID_CAPACITY, VALID_FUEL, VALID_BODY, VALID_TYPE);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("Manufacturer");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("VeryLongManufacturerNameThatExceedsTheMaximumAllowedLengthOfOneHundredCharactersDefinitelyByTheWayWhyAreYouStillReadingThisVeryVeryLongManufacturerNameGoMakeSomeCoffe")]
    public void Create_Should_ReturnError_WhenModelIsInvalid(string? invalidModel)
    {
        // Act
        var result = Vehicle.Create(_validOwnerId, VALID_VIN, VALID_MANUFACTURER, invalidModel, VALID_YEAR, VALID_CAPACITY, VALID_FUEL, VALID_BODY, VALID_TYPE);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("Model");
    }

    [Theory]
    [InlineData("1885")]
    [InlineData("2101")]
    [InlineData("NineteenNinety")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_Should_ReturnError_WhenProductionYearIsInvalid(string? invalidYear)
    {
        // Act:
        var result = Vehicle.Create(_validOwnerId, VALID_VIN, VALID_MANUFACTURER, VALID_MODEL, invalidYear, VALID_CAPACITY, VALID_FUEL, VALID_BODY, VALID_TYPE);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("Year");
    }

    [Theory]
    [InlineData(EFuelType.None)]
    [InlineData((EFuelType)99999)]
    public void Create_Should_ReturnError_WhenFuelTypeIsInvalid(EFuelType invalidFuel)
    {
        // Act
        var result = Vehicle.Create(_validOwnerId, VALID_VIN, VALID_MANUFACTURER, VALID_MODEL, VALID_YEAR, VALID_CAPACITY, invalidFuel, VALID_BODY, VALID_TYPE);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("fuel type");
    }

     [Theory]
    [InlineData(EBodyType.None)]
    [InlineData((EBodyType)99)]
    public void Create_Should_ReturnError_WhenBodyTypeIsInvalid(EBodyType invalidBody)
    {
        // Act
        var result = Vehicle.Create(_validOwnerId, VALID_VIN, VALID_MANUFACTURER, VALID_MODEL, VALID_YEAR, VALID_CAPACITY, VALID_FUEL, invalidBody, VALID_TYPE);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("body type");
    }

     [Theory]
    [InlineData(EVehicleType.None)]
    [InlineData((EVehicleType)99)]
    public void Create_Should_ReturnError_WhenVehicleTypeIsInvalid(EVehicleType invalidType)
    {
        // Act
        var result = Vehicle.Create(_validOwnerId, VALID_VIN, VALID_MANUFACTURER, VALID_MODEL, VALID_YEAR, VALID_CAPACITY, VALID_FUEL, VALID_BODY, invalidType);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("vehicle type");
    }

    [Fact]
    public void Create_Should_ReturnError_WhenEngineCapacityIsInvalid()
    {
        // Act
        var result = Vehicle.Create(_validOwnerId, VALID_VIN, VALID_MANUFACTURER, VALID_MODEL, VALID_YEAR, -1.0m, VALID_FUEL, VALID_BODY, VALID_TYPE);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("Engine capacity");
    }

    [Theory]
    [InlineData(EFuelType.None)]
    [InlineData(EBodyType.None)]
    [InlineData(EVehicleType.None)]
    public void Create_Should_ReturnError_WhenEnumTypeIsNone(object invalidEnum)
    {
        // Arrange
        EFuelType fuel = invalidEnum is EFuelType f ? f : VALID_FUEL;
        EBodyType body = invalidEnum is EBodyType b ? b : VALID_BODY;
        EVehicleType type = invalidEnum is EVehicleType t ? t : VALID_TYPE;

        // Act
        var result = Vehicle.Create(_validOwnerId, VALID_VIN, VALID_MANUFACTURER, VALID_MODEL, VALID_YEAR, VALID_CAPACITY, fuel, body, type);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("Invalid");
    }


    [Fact]
    public void ChangeOwner_Should_UpdateOwnerIdAndRaiseEvent_WhenNewIdIsValid()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        var newOwnerId = Guid.NewGuid();
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.ChangeOwner(newOwnerId);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.OwnerUserId.Should().Be(newOwnerId);
        vehicle.GetDomainEvents().Should().ContainSingle(e => e is VehicleOwnerChangedDomainEvent);
        var domainEvent = (VehicleOwnerChangedDomainEvent)vehicle.GetDomainEvents().First();
        domainEvent.Id.Should().Be(vehicle.Id);
        domainEvent.NewOwnerUserId.Should().Be(newOwnerId);
        domainEvent.OldOwnerId.Should().Be(_validOwnerId);
    }

    [Fact]
    public void ChangeOwner_Should_ReturnSuccess_AndNotRaiseEvent_WhenNewIdIsSame()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.ChangeOwner(_validOwnerId);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.OwnerUserId.Should().Be(_validOwnerId);
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void ChangeOwner_Should_ReturnError_WhenNewIdIsEmpty()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.ChangeOwner(Guid.Empty);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        vehicle.OwnerUserId.Should().Be(_validOwnerId);
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void UpdateVin_Should_UpdateVinAndRaiseEvent_WhenNewVinIsValid()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        var newVinInput = "WP0ZZZ99ZTS392124";
        var expectedNewVin = Vin.Create(newVinInput).Value!;
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.UpdateVin(newVinInput);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.Vin.Should().Be(expectedNewVin);
        vehicle.GetDomainEvents().Should().ContainSingle(e => e is VehicleVinChangedDomainEvent);
        var domainEvent = (VehicleVinChangedDomainEvent)vehicle.GetDomainEvents().First();
        domainEvent.Id.Should().Be(vehicle.Id);
        domainEvent.Vin.Should().Be(expectedNewVin);
        domainEvent.OldVin.Value.Should().Be(VALID_VIN);
    }

    [Fact]
    public void UpdateVin_Should_ReturnError_WhenNewVinIsInvalid()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        var invalidVinInput = "INVALID";
        var originalVin = vehicle.Vin;
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.UpdateVin(invalidVinInput);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        vehicle.Vin.Should().Be(originalVin);
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void UpdateEngineCapacity_Should_SetToNullAndRaiseEvent_WhenNewValueIsNull()
    {
         // Arrange
        var vehicle = CreateValidVehicle();
        vehicle.EngineCapacity.Should().NotBeNull();
        var oldCapacity = vehicle.EngineCapacity;
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.UpdateEngineCapacity(null);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.EngineCapacity.Should().BeNull();
        vehicle.GetDomainEvents().Should().ContainSingle(e => e is VehicleEngineCapacityChangedDomainEvent);
        var domainEvent = (VehicleEngineCapacityChangedDomainEvent)vehicle.GetDomainEvents().First();
        domainEvent.OldCapacity.Should().Be(oldCapacity);
        domainEvent.NewCapacity.Should().BeNull();
    }

    [Fact]
    public void ChangeFuelType_Should_UpdateTypeAndRaiseEvent_WhenNewTypeIsValid()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        var newFuelType = EFuelType.Electric;
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.ChangeFuelType(newFuelType);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.FuelType.Should().Be(newFuelType);
        vehicle.GetDomainEvents().Should().ContainSingle(e => e is VehicleFuelTypeChangedDomainEvent);
        var domainEvent = (VehicleFuelTypeChangedDomainEvent)vehicle.GetDomainEvents().First();
        domainEvent.OldFuelType.Should().Be(VALID_FUEL);
        domainEvent.NewFuelType.Should().Be(newFuelType);
    }

     [Fact]
    public void ChangeFuelType_Should_ReturnError_WhenNewTypeIsNone()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.ChangeFuelType(EFuelType.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        vehicle.FuelType.Should().Be(VALID_FUEL);
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void UpdateManufacturer_Should_UpdateManufacturerAndRaiseEvent_WhenNewManufacturerIsValid()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        var newManufacturerInput = "Audi";
        var expectedNewManufacturerResult = Manufacturer.Create(newManufacturerInput);
        expectedNewManufacturerResult.HasError().Should().BeFalse();
        var expectedNewManufacturer = expectedNewManufacturerResult.Value!;

        // Act
        var result = vehicle.UpdateManufacturer(newManufacturerInput);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.Manufacturer.Should().Be(expectedNewManufacturer);
        vehicle.GetDomainEvents().Should().ContainSingle(e => e is VehicleManufacturerChangedDomainEvent);
        var domainEvent = vehicle.GetDomainEvents().OfType<VehicleManufacturerChangedDomainEvent>().Single();
        domainEvent.Id.Should().Be(vehicle.Id);
        domainEvent.Manufacturer.Should().Be(expectedNewManufacturer);
        domainEvent.OldManufacturer.Value.Should().Be(VALID_MANUFACTURER);
    }

    [Fact]
    public void UpdateManufacturer_Should_ReturnSuccess_AndNotRaiseEvent_WhenManufacturerIsSame()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.UpdateManufacturer(VALID_MANUFACTURER);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.Manufacturer.Value.Should().Be(VALID_MANUFACTURER);
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdateManufacturer_Should_ReturnError_WhenNewManufacturerIsInvalid(string? invalidInput)
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        var originalManufacturer = vehicle.Manufacturer;
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.UpdateManufacturer(invalidInput);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        vehicle.Manufacturer.Should().Be(originalManufacturer);
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void UpdateModel_Should_UpdateModelAndRaiseEvent_WhenNewModelIsValid()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        var newModelInput = "A4";
        var expectedNewModelResult = Model.Create(newModelInput);
        expectedNewModelResult.HasError().Should().BeFalse();
        var expectedNewModel = expectedNewModelResult.Value!;

        // Act
        var result = vehicle.UpdateModel(newModelInput);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.Model.Should().Be(expectedNewModel);
        vehicle.GetDomainEvents().Should().ContainSingle(e => e is VehicleModelChangedDomainEvent);
        var domainEvent = vehicle.GetDomainEvents().OfType<VehicleModelChangedDomainEvent>().Single();
        domainEvent.Id.Should().Be(vehicle.Id);
        domainEvent.Model.Should().Be(expectedNewModel);
        domainEvent.OldModel.Value.Should().Be(VALID_MODEL);
    }

     [Fact]
    public void UpdateModel_Should_ReturnSuccess_AndNotRaiseEvent_WhenModelIsSame()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.UpdateModel(VALID_MODEL);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.Model.Value.Should().Be(VALID_MODEL);
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdateModel_Should_ReturnError_WhenNewModelIsInvalid(string? invalidInput)
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        var originalModel = vehicle.Model;
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.UpdateModel(invalidInput);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        vehicle.Model.Should().Be(originalModel);
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void UpdateProductionYear_Should_UpdateYearAndRaiseEvent_WhenNewYearIsValid()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        var newYearInput = "2024";
        var expectedNewYearResult = ProductionYear.Create(newYearInput);
        expectedNewYearResult.HasError().Should().BeFalse();
        var expectedNewYear = expectedNewYearResult.Value!;

        // Act
        var result = vehicle.UpdateProductionYear(newYearInput);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.ProductionYear.Should().Be(expectedNewYear);
        vehicle.GetDomainEvents().Should().ContainSingle(e => e is VehicleProductionYearChangedDomainEvent);
        var domainEvent = vehicle.GetDomainEvents().OfType<VehicleProductionYearChangedDomainEvent>().Single();
        domainEvent.Id.Should().Be(vehicle.Id);
        domainEvent.ProductionYear.Should().Be(expectedNewYear);
        domainEvent.OldProductionYear.Value.Should().Be(VALID_YEAR);
    }

    [Fact]
    public void UpdateProductionYear_Should_ReturnSuccess_AndNotRaiseEvent_WhenYearIsSame()
    {
        // Arrange
        var vehicle = CreateValidVehicle();

        // Act
        var result = vehicle.UpdateProductionYear(VALID_YEAR);
        vehicle.ClearDomainEvents();

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.ProductionYear.Value.Should().Be(VALID_YEAR);
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Theory]
    [InlineData("1800")]
    [InlineData(null)]
    [InlineData("abc")]
    public void UpdateProductionYear_Should_ReturnError_WhenNewYearIsInvalid(string? invalidInput)
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        var originalYear = vehicle.ProductionYear;
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.UpdateProductionYear(invalidInput);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        vehicle.ProductionYear.Should().Be(originalYear);
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void UpdateEngineCapacity_Should_UpdateCapacityAndRaiseEvent_WhenNewCapacityIsValid()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        decimal newCapacityValue = 2.5m;
        var expectedNewCapacityResult = EngineCapacity.Create(newCapacityValue);
        expectedNewCapacityResult.HasError().Should().BeFalse();
        var expectedNewCapacity = expectedNewCapacityResult.Value!;
        var oldCapacity = vehicle.EngineCapacity;

        // Act
        var result = vehicle.UpdateEngineCapacity(newCapacityValue);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.EngineCapacity.Should().Be(expectedNewCapacity);
        vehicle.GetDomainEvents().Should().ContainSingle(e => e is VehicleEngineCapacityChangedDomainEvent);
        var domainEvent = vehicle.GetDomainEvents().OfType<VehicleEngineCapacityChangedDomainEvent>().Single();
        domainEvent.Id.Should().Be(vehicle.Id);
        domainEvent.NewCapacity.Should().Be(expectedNewCapacity);
        domainEvent.OldCapacity.Should().Be(oldCapacity);
    }

    [Fact]
    public void UpdateEngineCapacity_Should_ReturnSuccess_AndNotRaiseEvent_WhenCapacityIsSame()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        var sameCapacityValue = VALID_CAPACITY;

        // Act
        var result = vehicle.UpdateEngineCapacity(sameCapacityValue);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.EngineCapacity!.Value.Should().Be(VALID_CAPACITY);
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1.0)]
    public void UpdateEngineCapacity_Should_ReturnError_WhenNewCapacityIsInvalid(decimal invalidCapacity)
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        var originalCapacity = vehicle.EngineCapacity;
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.UpdateEngineCapacity(invalidCapacity);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        vehicle.EngineCapacity.Should().Be(originalCapacity);
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void ChangeFuelType_Should_ReturnSuccess_AndNotRaiseEvent_WhenFuelTypeIsSame()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.ChangeFuelType(VALID_FUEL);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.FuelType.Should().Be(VALID_FUEL);
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void ChangeBodyType_Should_UpdateBodyTypeAndRaiseEvent_WhenNewTypeIsValid()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        var newBodyType = EBodyType.Kombi;

        // Act
        var result = vehicle.ChangeBodyType(newBodyType);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.BodyType.Should().Be(newBodyType);
        vehicle.GetDomainEvents().Should().ContainSingle(e => e is VehicleBodyTypeChangedDomainEvent);
        var domainEvent = vehicle.GetDomainEvents().OfType<VehicleBodyTypeChangedDomainEvent>().Single();
        domainEvent.Id.Should().Be(vehicle.Id);
        domainEvent.NewBodyType.Should().Be(newBodyType);
        domainEvent.OldBodyType.Should().Be(VALID_BODY);
    }

    [Fact]
    public void ChangeBodyType_Should_ReturnSuccess_AndNotRaiseEvent_WhenBodyTypeIsSame()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.ChangeBodyType(VALID_BODY);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.BodyType.Should().Be(VALID_BODY);
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Theory]
    [InlineData(EBodyType.None)]
    [InlineData((EBodyType)99)]
    public void ChangeBodyType_Should_ReturnError_WhenNewTypeIsInvalid(EBodyType invalidType)
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        vehicle.ClearDomainEvents();
        var originalType = vehicle.BodyType;

        // Act
        var result = vehicle.ChangeBodyType(invalidType);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        vehicle.BodyType.Should().Be(originalType);
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void ChangeVehicleType_Should_UpdateVehicleTypeAndRaiseEvent_WhenNewTypeIsValid()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        var newVehicleType = EVehicleType.Motorcycle;

        // Act
        var result = vehicle.ChangeVehicleType(newVehicleType);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.VehicleType.Should().Be(newVehicleType);
        vehicle.GetDomainEvents().Should().ContainSingle(e => e is VehicleTypeChangedDomainEvent);
        var domainEvent = vehicle.GetDomainEvents().OfType<VehicleTypeChangedDomainEvent>().Single();
        domainEvent.Id.Should().Be(vehicle.Id);
        domainEvent.NewVehicleType.Should().Be(newVehicleType);
        domainEvent.OldVehicleType.Should().Be(VALID_TYPE);
    }

    [Fact]
    public void ChangeVehicleType_Should_ReturnSuccess_AndNotRaiseEvent_WhenVehicleTypeIsSame()
    {
        // Arrange
        var vehicle = CreateValidVehicle();

        // Act
        var result = vehicle.ChangeVehicleType(VALID_TYPE);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.VehicleType.Should().Be(VALID_TYPE);
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Theory]
    [InlineData(EVehicleType.None)]
    [InlineData((EVehicleType)99)]
    public void ChangeVehicleType_Should_ReturnError_WhenNewTypeIsInvalid(EVehicleType invalidType)
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        vehicle.ClearDomainEvents();
        var originalType = vehicle.VehicleType;

        // Act
        var result = vehicle.ChangeVehicleType(invalidType);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        vehicle.VehicleType.Should().Be(originalType);
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

}

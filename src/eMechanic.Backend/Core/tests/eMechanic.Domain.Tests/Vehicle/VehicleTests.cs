namespace eMechanic.Domain.Tests.Vehicle;

using Domain.Vehicle;
using Domain.Vehicle.ValueObjects;
using Domain.Vehicle.Enums;
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
    private const int VALID_MILEAGE_VALUE = 150000;
    private const EMileageUnit VALID_MILEAGE_UNIT = EMileageUnit.Kilometers;
    private const string VALID_LICENSE_PLATE = "PZ1W924";
    private const int VALID_HORSE_POWER = 240;
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
            VALID_MILEAGE_VALUE,
            VALID_MILEAGE_UNIT,
            VALID_LICENSE_PLATE,
            VALID_HORSE_POWER,
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
            VALID_MILEAGE_VALUE,
            VALID_MILEAGE_UNIT,
            VALID_LICENSE_PLATE,
            VALID_HORSE_POWER,
            VALID_FUEL,
            VALID_BODY,
            VALID_TYPE);

        // Assert
        result.HasError().Should().BeFalse();
        var vehicle = result.Value;
        vehicle.Should().NotBeNull();
        vehicle.Id.Should().NotBeEmpty();
        vehicle.UserId.Should().Be(_validOwnerId);
        vehicle.Vin.Value.Should().Be(VALID_VIN);
        vehicle.Manufacturer.Value.Should().Be(VALID_MANUFACTURER);
        vehicle.Model.Value.Should().Be(VALID_MODEL);
        vehicle.ProductionYear.Value.Should().Be(VALID_YEAR);
        vehicle.EngineCapacity.Should().NotBeNull();
        vehicle.EngineCapacity!.Value.Should().Be(VALID_CAPACITY);
        vehicle.FuelType.Should().Be(VALID_FUEL);
        vehicle.BodyType.Should().Be(VALID_BODY);
        vehicle.VehicleType.Should().Be(VALID_TYPE);
        vehicle.LicensePlate.Value.Should().Be(VALID_LICENSE_PLATE);
        vehicle.HorsePower.Value.Should().Be(VALID_HORSE_POWER);

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
            VALID_MILEAGE_VALUE,
            VALID_MILEAGE_UNIT,
            VALID_LICENSE_PLATE,
            VALID_HORSE_POWER,
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
        var result = Vehicle.Create(
            Guid.Empty,
            VALID_VIN,
            VALID_MANUFACTURER,
            VALID_MODEL,
            VALID_YEAR,
            VALID_CAPACITY,
            VALID_MILEAGE_VALUE,
            VALID_MILEAGE_UNIT,
            VALID_LICENSE_PLATE,
            VALID_HORSE_POWER,
            VALID_FUEL,
            VALID_BODY,
            VALID_TYPE);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("userId");
    }

    [Theory]
    [InlineData("INVALIDVINLEN")]
    [InlineData("V!N123456789ABCDE")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_Should_ReturnError_WhenVinIsInvalid(string? invalidVin)
    {
        var result = Vehicle.Create(
            _validOwnerId,
            invalidVin,
            VALID_MANUFACTURER,
            VALID_MODEL,
            VALID_YEAR,
            VALID_CAPACITY,
            VALID_MILEAGE_VALUE,
            VALID_MILEAGE_UNIT,
            VALID_LICENSE_PLATE,
            VALID_HORSE_POWER,
            VALID_FUEL,
            VALID_BODY,
            VALID_TYPE);

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
        var result = Vehicle.Create(
            _validOwnerId,
            VALID_VIN,
            invalidManufacturer,
            VALID_MODEL,
            VALID_YEAR,
            VALID_CAPACITY,
            VALID_MILEAGE_VALUE,
            VALID_MILEAGE_UNIT,
            VALID_LICENSE_PLATE,
            VALID_HORSE_POWER,
            VALID_FUEL,
            VALID_BODY,
            VALID_TYPE);

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
        var result = Vehicle.Create(
            _validOwnerId,
            VALID_VIN,
            VALID_MANUFACTURER,
            invalidModel,
            VALID_YEAR,
            VALID_CAPACITY,
            VALID_MILEAGE_VALUE,
            VALID_MILEAGE_UNIT,
            VALID_LICENSE_PLATE,
            VALID_HORSE_POWER,
            VALID_FUEL,
            VALID_BODY,
            VALID_TYPE);

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
        var result = Vehicle.Create(
            _validOwnerId,
            VALID_VIN,
            VALID_MANUFACTURER,
            VALID_MODEL,
            invalidYear,
            VALID_CAPACITY,
            VALID_MILEAGE_VALUE,
            VALID_MILEAGE_UNIT,
            VALID_LICENSE_PLATE,
            VALID_HORSE_POWER,
            VALID_FUEL,
            VALID_BODY,
            VALID_TYPE);

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
        var result = Vehicle.Create(
            _validOwnerId,
            VALID_VIN,
            VALID_MANUFACTURER,
            VALID_MODEL,
            VALID_YEAR,
            VALID_CAPACITY,
            VALID_MILEAGE_VALUE,
            VALID_MILEAGE_UNIT,
            VALID_LICENSE_PLATE,
            VALID_HORSE_POWER,
            invalidFuel,
            VALID_BODY,
            VALID_TYPE);

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
        var result = Vehicle.Create(
            _validOwnerId,
            VALID_VIN,
            VALID_MANUFACTURER,
            VALID_MODEL,
            VALID_YEAR,
            VALID_CAPACITY,
            VALID_MILEAGE_VALUE,
            VALID_MILEAGE_UNIT,
            VALID_LICENSE_PLATE,
            VALID_HORSE_POWER,
            VALID_FUEL,
            invalidBody,
            VALID_TYPE);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
    }

    [Theory]
    [InlineData(EVehicleType.None)]
    [InlineData((EVehicleType)99)]
    public void Create_Should_ReturnError_WhenVehicleTypeIsInvalid(EVehicleType invalidType)
    {
        // Act
        var result = Vehicle.Create(
            _validOwnerId,
            VALID_VIN,
            VALID_MANUFACTURER,
            VALID_MODEL,
            VALID_YEAR,
            VALID_CAPACITY,
            VALID_MILEAGE_VALUE,
            VALID_MILEAGE_UNIT,
            VALID_LICENSE_PLATE,
            VALID_HORSE_POWER,
            VALID_FUEL,
            VALID_BODY,
            invalidType);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
    }

    [Fact]
    public void Create_Should_ReturnError_WhenEngineCapacityIsInvalid()
    {
        // Act
        var result = Vehicle.Create(
            _validOwnerId,
            VALID_VIN,
            VALID_MANUFACTURER,
            VALID_MODEL,
            VALID_YEAR,
            -1.0m,
            VALID_MILEAGE_VALUE,
            VALID_MILEAGE_UNIT,
            VALID_LICENSE_PLATE,
            VALID_HORSE_POWER,
            VALID_FUEL,
            VALID_BODY,
            VALID_TYPE);

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
        var result = Vehicle.Create(
            _validOwnerId,
            VALID_VIN,
            VALID_MANUFACTURER,
            VALID_MODEL,
            VALID_YEAR,
            VALID_CAPACITY,
            VALID_MILEAGE_VALUE,
            VALID_MILEAGE_UNIT,
            VALID_LICENSE_PLATE,
            VALID_HORSE_POWER,
            fuel,
            body,
            type);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
    }

    [Fact]
    public void Create_Should_ReturnError_WhenMileageIsInvalid()
    {
        // Act
        var result = Vehicle.Create(
            _validOwnerId,
            VALID_VIN,
            VALID_MANUFACTURER,
            VALID_MODEL,
            VALID_YEAR,
            VALID_CAPACITY,
            -100,
            VALID_MILEAGE_UNIT,
            VALID_LICENSE_PLATE,
            VALID_HORSE_POWER,
            VALID_FUEL,
            VALID_BODY,
            VALID_TYPE);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
    }

    [Fact]
    public void Create_Should_ReturnError_WhenMileageUnitIsInvalid()
    {
        // Act
        var result = Vehicle.Create(
            _validOwnerId,
            VALID_VIN,
            VALID_MANUFACTURER,
            VALID_MODEL,
            VALID_YEAR,
            VALID_CAPACITY,
            VALID_MILEAGE_VALUE,
            EMileageUnit.None,
            VALID_LICENSE_PLATE,
            VALID_HORSE_POWER,
            VALID_FUEL,
            VALID_BODY,
            VALID_TYPE);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
    }


    [Fact]
    public void ChangeOwner_Should_UpdateOwnerIdAndRaiseEvent_WhenNewIdIsValid()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        var newOwnerId = Guid.NewGuid();
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.ChangeUserId(newOwnerId);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.UserId.Should().Be(newOwnerId);
        vehicle.GetDomainEvents().Should().ContainSingle(e => e is VehicleUserIdChangedDomainEvent);
        var domainEvent = (VehicleUserIdChangedDomainEvent)vehicle.GetDomainEvents().First();
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
        var result = vehicle.ChangeUserId(_validOwnerId);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.UserId.Should().Be(_validOwnerId);
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void ChangeOwner_Should_ReturnError_WhenNewIdIsEmpty()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.ChangeUserId(Guid.Empty);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        vehicle.UserId.Should().Be(_validOwnerId);
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
        vehicle.ClearDomainEvents();

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
    public void UpdateMileage_Should_UpdateMileageAndRaiseEvent_WhenNewValueIsValid()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        vehicle.ClearDomainEvents();

        var newMileageValue = VALID_MILEAGE_VALUE + 100;
        var newMileageUnit = EMileageUnit.Kilometers;
        var oldMileage = vehicle.Mileage;

        // Act
        var result = vehicle.UpdateMileage(newMileageValue, newMileageUnit);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.Mileage.Value.Should().Be(newMileageValue);
        vehicle.Mileage.Unit.Should().Be(newMileageUnit);

        vehicle.GetDomainEvents().Should().ContainSingle(e => e is VehicleMileageChangedDomainEvent);
        var domainEvent = (VehicleMileageChangedDomainEvent)vehicle.GetDomainEvents().First();

        domainEvent.Id.Should().Be(vehicle.Id);
        domainEvent.OldMileage.Should().Be(oldMileage);
        domainEvent.Mileage.Should().Be(vehicle.Mileage);
    }

    [Fact]
    public void UpdateMileage_Should_ReturnError_WhenNewValueIsInvalid()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        var originalMileage = vehicle.Mileage;
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.UpdateMileage(-50, EMileageUnit.Kilometers);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        vehicle.Mileage.Should().Be(originalMileage);
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void UpdateMileage_Should_ReturnSuccess_AndNotRaiseEvent_WhenValueIsSame()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        var sameMileageValue = vehicle.Mileage.Value;
        var sameMileageUnit = vehicle.Mileage.Unit;
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.UpdateMileage(sameMileageValue, sameMileageUnit);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void Create_Should_ReturnSuccess_WhenVehicleTypeIsMotorcycleAndBodyTypeIsNone()
    {
        // Act
        var result = Vehicle.Create(
            _validOwnerId,
            VALID_VIN,
            VALID_MANUFACTURER,
            VALID_MODEL,
            VALID_YEAR,
            null,
            VALID_MILEAGE_VALUE,
            VALID_MILEAGE_UNIT,
            VALID_LICENSE_PLATE,
            VALID_HORSE_POWER,
            VALID_FUEL,
            EBodyType.None,
            EVehicleType.Motorcycle
        );

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.VehicleType.Should().Be(EVehicleType.Motorcycle);
        result.Value.BodyType.Should().Be(EBodyType.None);
    }

    [Fact]
    public void Create_Should_ReturnError_WhenVehicleTypeIsMotorcycleAndBodyTypeIsNotNone()
    {
        // Act
        var result = Vehicle.Create(
            _validOwnerId,
            VALID_VIN,
            VALID_MANUFACTURER,
            VALID_MODEL,
            VALID_YEAR,
            null,
            VALID_MILEAGE_VALUE,
            VALID_MILEAGE_UNIT,
            VALID_LICENSE_PLATE,
            VALID_HORSE_POWER,
            VALID_FUEL,
            EBodyType.Hatchback,
            EVehicleType.Motorcycle
        );

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("BodyType must be None when VehicleType is Motorcycle");
    }

    [Fact]
    public void Create_Should_ReturnError_WhenVehicleTypeIsNotMotorcycleAndBodyTypeIsNone()
    {
        // Act
        var result = Vehicle.Create(
            _validOwnerId,
            VALID_VIN,
            VALID_MANUFACTURER,
            VALID_MODEL,
            VALID_YEAR,
            null,
            VALID_MILEAGE_VALUE,
            VALID_MILEAGE_UNIT,
            VALID_LICENSE_PLATE,
            VALID_HORSE_POWER,
            VALID_FUEL,
            EBodyType.None,
            EVehicleType.Passenger
        );

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("BodyType must be specified");
    }

    [Fact]
    public void UpdateClassification_Should_ReturnError_WhenChangingPassengerToMotorcycleButKeepingSedan()
    {
        // Arrange
        var vehicle = CreateValidVehicle();

        // Act
        var result = vehicle.UpdateClassification(EBodyType.Sedan, EVehicleType.Motorcycle);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Message.Should().Contain("BodyType must be None when VehicleType is Motorcycle");
    }

    [Fact]
    public void UpdateClassification_Should_ReturnError_WhenChangingPassengerToPassengerButSettingBodyToNone()
    {
        // Arrange
        var vehicle = CreateValidVehicle();

        // Act
        var result = vehicle.UpdateClassification(EBodyType.None, EVehicleType.Passenger);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Message.Should().Contain("BodyType must be specified");
    }

    [Fact]
    public void UpdateClassification_Should_ReturnSuccess_WhenChangingPassengerSedanToMotorcycleNone()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.UpdateClassification(EBodyType.None, EVehicleType.Motorcycle);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.VehicleType.Should().Be(EVehicleType.Motorcycle);
        vehicle.BodyType.Should().Be(EBodyType.None);

        vehicle.GetDomainEvents().Count.Should().Be(2);
        vehicle.GetDomainEvents().Should().ContainItemsAssignableTo<VehicleBodyTypeChangedDomainEvent>();
        vehicle.GetDomainEvents().Should().ContainItemsAssignableTo<VehicleTypeChangedDomainEvent>();
    }

    [Fact]
    public void UpdateClassification_Should_ReturnSuccess_WhenChangingSedanToKombi()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.UpdateClassification(EBodyType.Kombi, EVehicleType.Passenger);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.VehicleType.Should().Be(EVehicleType.Passenger);
        vehicle.BodyType.Should().Be(EBodyType.Kombi);

        vehicle.GetDomainEvents().Count.Should().Be(1);
        vehicle.GetDomainEvents().Should().ContainItemsAssignableTo<VehicleBodyTypeChangedDomainEvent>();
    }


    [Fact]
    public void Create_Should_ReturnError_WhenLicensePlateIsInvalid()
    {
        // Act
        var result = Vehicle.Create(
            _validOwnerId, VALID_VIN, VALID_MANUFACTURER, VALID_MODEL, VALID_YEAR, VALID_CAPACITY,
            VALID_MILEAGE_VALUE, VALID_MILEAGE_UNIT,
            "INVALID PLATE !!!",
            VALID_HORSE_POWER, VALID_FUEL, VALID_BODY, VALID_TYPE);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
    }

    [Fact]
    public void Create_Should_ReturnError_WhenHorsePowerIsInvalid()
    {
        // Act
        var result = Vehicle.Create(
            _validOwnerId, VALID_VIN, VALID_MANUFACTURER, VALID_MODEL, VALID_YEAR, VALID_CAPACITY,
            VALID_MILEAGE_VALUE, VALID_MILEAGE_UNIT, VALID_LICENSE_PLATE,
            -50,
            VALID_FUEL, VALID_BODY, VALID_TYPE);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("HorsePower must be a positive value");
    }

    [Fact]
    public void UpdateLicensePlate_Should_UpdatePlateAndRaiseEvent_WhenNewPlateIsValid()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        var newPlateInput = "NEW 001";
        var expectedNewPlate = LicensePlate.Create(newPlateInput).Value!;
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.UpdateLicensePlate(newPlateInput);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.LicensePlate.Should().Be(expectedNewPlate);
        vehicle.GetDomainEvents().Should().ContainSingle(e => e is VehicleLicensePlateChangedDomainEvent);
    }

    [Fact]
    public void UpdateLicensePlate_Should_ReturnError_WhenNewPlateIsInvalid()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        var invalidPlateInput = "BAD!";
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.UpdateLicensePlate(invalidPlateInput);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Message.Should().Contain("invalid characters");
        vehicle.LicensePlate.Value.Should().Be(VALID_LICENSE_PLATE);
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void UpdateHorsePower_Should_UpdateHpAndRaiseEvent_WhenNewHpIsValid()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        var newHpInput = 200;
        var expectedNewHp = HorsePower.Create(newHpInput).Value!;
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.UpdateHorsePower(newHpInput);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.HorsePower.Should().Be(expectedNewHp);
        vehicle.GetDomainEvents().Should().ContainSingle(e => e is VehicleHorsePowerChangedDomainEvent);
    }

    [Fact]
    public void UpdateHorsePower_Should_ReturnError_WhenNewHpIsInvalid()
    {
        // Arrange
        var vehicle = CreateValidVehicle();
        var invalidHpInput = -10;
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.UpdateHorsePower(invalidHpInput);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Message.Should().Contain("positive value");
        vehicle.HorsePower.Value.Should().Be(VALID_HORSE_POWER);
        vehicle.GetDomainEvents().Should().BeEmpty();
    }
}

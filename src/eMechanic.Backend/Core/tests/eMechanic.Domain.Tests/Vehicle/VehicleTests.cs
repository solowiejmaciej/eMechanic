namespace eMechanic.Domain.Tests.Vehicle;

using System.Globalization;
using Builders;
using Common.Result;
using Domain.Vehicle;
using Domain.Vehicle.Enums;
using Domain.Vehicle.ValueObjects;
using FluentAssertions;

public class VehicleTests
{
    private readonly Guid _validOwnerId = Guid.NewGuid();

    [Fact]
    public void Create_Should_ReturnSuccessAndVehicle_WhenAllDataIsValid()
    {
        // Arrange
        var builder = new VehicleBuilder();

        // Act
        var result = builder.WithOwnerId(_validOwnerId).BuildResult();

        // Assert
        result.HasError().Should().BeFalse();
        var vehicle = result.Value;
        vehicle.Should().NotBeNull();
        vehicle.Id.Should().NotBeEmpty();
        vehicle.UserId.Should().Be(_validOwnerId);
        vehicle.Vin.Value.Should().NotBeNullOrWhiteSpace();
        vehicle.Manufacturer.Value.Should().NotBeNullOrWhiteSpace();
        vehicle.Model.Value.Should().NotBeNullOrWhiteSpace();
        vehicle.ProductionYear.Value.Should().NotBeNullOrWhiteSpace();
        vehicle.EngineCapacity.Should().NotBeNull();
        vehicle.FuelType.Should().NotBe(EFuelType.None);
        vehicle.BodyType.Should().NotBe(EBodyType.None);
        vehicle.VehicleType.Should().NotBe(EVehicleType.None);
        vehicle.LicensePlate.Value.Should().NotBeNullOrWhiteSpace();
        vehicle.HorsePower.Value.Should().BePositive();

        vehicle.GetDomainEvents().Should().ContainSingle(e => e is VehicleCreatedDomainEvent);
        var domainEvent = (VehicleCreatedDomainEvent)vehicle.GetDomainEvents().First();
        domainEvent.Vehicle.Id.Should().Be(vehicle.Id);
    }

    [Fact]
    public void Create_Should_ReturnSuccess_WhenEngineCapacityIsNull()
    {
        // Arrange
        var builder = new VehicleBuilder().WithEngineCapacity(null);

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.EngineCapacity.Should().BeNull();
    }

    [Fact]
    public void Create_Should_ReturnError_WhenOwnerIdIsEmpty()
    {
        // Arrange
        var builder = new VehicleBuilder().WithOwnerId(Guid.Empty);

        // Act
        var result = builder.BuildResult();

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
        // Arrange
        var builder = new VehicleBuilder().WithVin(invalidVin!);

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("VIN");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData(
        "VeryLongManufacturerNameThatExceedsTheMaximumAllowedLengthOfOneHundredCharactersDefinitelyByTheWayWhyAreYouStillReadingThisVeryVeryLongManufacturerNameGoMakeSomeCoffe")]
    public void Create_Should_ReturnError_WhenManufacturerIsInvalid(string? invalidManufacturer)
    {
        // Arrange
        var builder = new VehicleBuilder().WithManufacturer(invalidManufacturer!);

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("Manufacturer");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData(
        "VeryLongManufacturerNameThatExceedsTheMaximumAllowedLengthOfOneHundredCharactersDefinitelyByTheWayWhyAreYouStillReadingThisVeryVeryLongManufacturerNameGoMakeSomeCoffe")]
    public void Create_Should_ReturnError_WhenModelIsInvalid(string? invalidModel)
    {
        // Arrange
        var builder = new VehicleBuilder().WithModel(invalidModel!);

        // Act
        var result = builder.BuildResult();

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
        // Arrange
        var builder = new VehicleBuilder().WithProductionYear(invalidYear!);

        // Act:
        var result = builder.BuildResult();

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
        // Arrange
        var builder = new VehicleBuilder().WithFuelType(invalidFuel);

        // Act
        var result = builder.BuildResult();

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
        // Arrange
        var builder = new VehicleBuilder().WithBodyType(invalidBody);

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
    }

    [Theory]
    [InlineData(EVehicleType.None)]
    [InlineData((EVehicleType)99)]
    public void Create_Should_ReturnError_WhenVehicleTypeIsInvalid(EVehicleType invalidType)
    {
        // Arrange
        var builder = new VehicleBuilder().WithVehicleType(invalidType);

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
    }

    [Fact]
    public void Create_Should_ReturnError_WhenEngineCapacityIsInvalid()
    {
        // Arrange
        var builder = new VehicleBuilder().WithEngineCapacity(-1.0m);

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("Engine capacity");
    }

    [Fact]
    public void Create_Should_ReturnError_WhenMileageIsInvalid()
    {
        // Arrange
        var builder = new VehicleBuilder().WithMileage(-100, EMileageUnit.Kilometers);

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
    }

    [Fact]
    public void Create_Should_ReturnError_WhenMileageUnitIsInvalid()
    {
        // Arrange
        var builder = new VehicleBuilder().WithMileage(100, EMileageUnit.None);

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
    }


    [Fact]
    public void ChangeOwner_Should_UpdateOwnerIdAndRaiseEvent_WhenNewIdIsValid()
    {
        // Arrange
        var vehicle = new VehicleBuilder().WithOwnerId(_validOwnerId).Build();
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
        var vehicle = new VehicleBuilder().WithOwnerId(_validOwnerId).Build();
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
        var vehicle = new VehicleBuilder().WithOwnerId(_validOwnerId).Build();
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
        var oldVin = "JMZGG128271672202";
        var vehicle = new VehicleBuilder().WithVin(oldVin).Build();
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
        domainEvent.OldVin.Value.Should().Be(oldVin);
    }

    [Fact]
    public void UpdateVin_Should_ReturnError_WhenNewVinIsInvalid()
    {
        // Arrange
        var vehicle = new VehicleBuilder().Build();
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
        var vehicle = new VehicleBuilder().WithEngineCapacity(1.9m).Build();
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
        var oldFuelType = EFuelType.Gasoline;
        var vehicle = new VehicleBuilder().WithFuelType(oldFuelType).Build();
        var newFuelType = EFuelType.Electric;
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.ChangeFuelType(newFuelType);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.FuelType.Should().Be(newFuelType);
        vehicle.GetDomainEvents().Should().ContainSingle(e => e is VehicleFuelTypeChangedDomainEvent);
        var domainEvent = (VehicleFuelTypeChangedDomainEvent)vehicle.GetDomainEvents().First();
        domainEvent.OldFuelType.Should().Be(oldFuelType);
        domainEvent.NewFuelType.Should().Be(newFuelType);
    }

    [Fact]
    public void ChangeFuelType_Should_ReturnError_WhenNewTypeIsNone()
    {
        // Arrange
        var vehicle = new VehicleBuilder().Build();
        var originalFuelType = vehicle.FuelType;
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.ChangeFuelType(EFuelType.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        vehicle.FuelType.Should().Be(originalFuelType);
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void UpdateManufacturer_Should_UpdateManufacturerAndRaiseEvent_WhenNewManufacturerIsValid()
    {
        // Arrange
        var oldManufacturer = "Mazda";
        var vehicle = new VehicleBuilder().WithManufacturer(oldManufacturer).Build();
        var newManufacturerInput = "Audi";
        var expectedNewManufacturer = Manufacturer.Create(newManufacturerInput).Value!;

        // Act
        var result = vehicle.UpdateManufacturer(newManufacturerInput);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.Manufacturer.Should().Be(expectedNewManufacturer);
        vehicle.GetDomainEvents().Should().ContainSingle(e => e is VehicleManufacturerChangedDomainEvent);
        var domainEvent = vehicle.GetDomainEvents().OfType<VehicleManufacturerChangedDomainEvent>().Single();
        domainEvent.Id.Should().Be(vehicle.Id);
        domainEvent.Manufacturer.Should().Be(expectedNewManufacturer);
        domainEvent.OldManufacturer.Value.Should().Be(oldManufacturer);
    }

    [Fact]
    public void UpdateManufacturer_Should_ReturnSuccess_AndNotRaiseEvent_WhenManufacturerIsSame()
    {
        // Arrange
        var vehicle = new VehicleBuilder().Build();
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.UpdateManufacturer(vehicle.Manufacturer.Value);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdateManufacturer_Should_ReturnError_WhenNewManufacturerIsInvalid(string? invalidInput)
    {
        // Arrange
        var vehicle = new VehicleBuilder().Build();
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
        var oldModel = "6";
        var vehicle = new VehicleBuilder().WithModel(oldModel).Build();
        var newModelInput = "A4";
        var expectedNewModel = Model.Create(newModelInput).Value!;

        // Act
        var result = vehicle.UpdateModel(newModelInput);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.Model.Should().Be(expectedNewModel);
        vehicle.GetDomainEvents().Should().ContainSingle(e => e is VehicleModelChangedDomainEvent);
        var domainEvent = vehicle.GetDomainEvents().OfType<VehicleModelChangedDomainEvent>().Single();
        domainEvent.Id.Should().Be(vehicle.Id);
        domainEvent.Model.Should().Be(expectedNewModel);
        domainEvent.OldModel.Value.Should().Be(oldModel);
    }

    [Fact]
    public void UpdateModel_Should_ReturnSuccess_AndNotRaiseEvent_WhenModelIsSame()
    {
        // Arrange
        var vehicle = new VehicleBuilder().Build();
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.UpdateModel(vehicle.Model.Value);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdateModel_Should_ReturnError_WhenNewModelIsInvalid(string? invalidInput)
    {
        // Arrange
        var vehicle = new VehicleBuilder().Build();
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
        var oldYear = "2006";
        var vehicle = new VehicleBuilder().WithProductionYear(oldYear).Build();
        var newYearInput = "2024";
        var expectedNewYear = ProductionYear.Create(newYearInput).Value!;

        // Act
        var result = vehicle.UpdateProductionYear(newYearInput);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.ProductionYear.Should().Be(expectedNewYear);
        vehicle.GetDomainEvents().Should().ContainSingle(e => e is VehicleProductionYearChangedDomainEvent);
        var domainEvent = vehicle.GetDomainEvents().OfType<VehicleProductionYearChangedDomainEvent>().Single();
        domainEvent.Id.Should().Be(vehicle.Id);
        domainEvent.ProductionYear.Should().Be(expectedNewYear);
        domainEvent.OldProductionYear.Value.Should().Be(oldYear);
    }

    [Fact]
    public void UpdateProductionYear_Should_ReturnSuccess_AndNotRaiseEvent_WhenYearIsSame()
    {
        // Arrange
        var vehicle = new VehicleBuilder().Build();
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.UpdateProductionYear(vehicle.ProductionYear.Value);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Theory]
    [InlineData("1800")]
    [InlineData(null)]
    [InlineData("abc")]
    public void UpdateProductionYear_Should_ReturnError_WhenNewYearIsInvalid(string? invalidInput)
    {
        // Arrange
        var vehicle = new VehicleBuilder().Build();
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
        var vehicle = new VehicleBuilder().WithEngineCapacity(1.9m).Build();
        decimal newCapacityValue = 2.5m;
        var expectedNewCapacity = EngineCapacity.Create(newCapacityValue).Value!;
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
        var vehicle = new VehicleBuilder().WithEngineCapacity(1.9m).Build();
        var sameCapacityValue = vehicle.EngineCapacity!.Value;
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.UpdateEngineCapacity(sameCapacityValue);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1.0)]
    public void UpdateEngineCapacity_Should_ReturnError_WhenNewCapacityIsInvalid(decimal invalidCapacity)
    {
        // Arrange
        var vehicle = new VehicleBuilder().Build();
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
        var vehicle = new VehicleBuilder().Build();
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.ChangeFuelType(vehicle.FuelType);

        // Assert
        result.HasError().Should().BeFalse();
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void UpdateMileage_Should_UpdateMileageAndRaiseEvent_WhenNewValueIsValid()
    {
        // Arrange
        var vehicle = new VehicleBuilder().Build();
        vehicle.ClearDomainEvents();

        var newMileageValue = vehicle.Mileage.Value + 100;
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
        var vehicle = new VehicleBuilder().Build();
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
        var vehicle = new VehicleBuilder().Build();
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
        // Arrange
        var builder = new VehicleBuilder()
            .WithVehicleType(EVehicleType.Motorcycle)
            .WithBodyType(EBodyType.None);

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.VehicleType.Should().Be(EVehicleType.Motorcycle);
        result.Value.BodyType.Should().Be(EBodyType.None);
    }

    [Fact]
    public void Create_Should_ReturnError_WhenVehicleTypeIsMotorcycleAndBodyTypeIsNotNone()
    {
        // Arrange
        var builder = new VehicleBuilder()
            .WithVehicleType(EVehicleType.Motorcycle)
            .WithBodyType(EBodyType.Hatchback);

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("BodyType must be None when VehicleType is Motorcycle");
    }

    [Fact]
    public void Create_Should_ReturnError_WhenVehicleTypeIsNotMotorcycleAndBodyTypeIsNone()
    {
        // Arrange
        var builder = new VehicleBuilder()
            .WithVehicleType(EVehicleType.Passenger)
            .WithBodyType(EBodyType.None);

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("BodyType must be specified");
    }

    [Fact]
    public void UpdateClassification_Should_ReturnError_WhenChangingPassengerToMotorcycleButKeepingSedan()
    {
        // Arrange
        var vehicle = new VehicleBuilder().Build();

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
        var vehicle = new VehicleBuilder().Build();

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
        var vehicle = new VehicleBuilder().Build();
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
        var vehicle = new VehicleBuilder().Build();
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
        // Arrange
        var builder = new VehicleBuilder().WithLicensePlate("INVALID PLATE !!!");

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
    }

    [Fact]
    public void Create_Should_ReturnError_WhenHorsePowerIsInvalid()
    {
        // Arrange
        var builder = new VehicleBuilder().WithHorsePower(-50);

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("HorsePower must be a positive value");
    }

    [Fact]
    public void UpdateLicensePlate_Should_UpdatePlateAndRaiseEvent_WhenNewPlateIsValid()
    {
        // Arrange
        var vehicle = new VehicleBuilder().Build();
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
        var vehicle = new VehicleBuilder().Build();
        var invalidPlateInput = "BAD!";
        var originalPlate = vehicle.LicensePlate;
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.UpdateLicensePlate(invalidPlateInput);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Message.Should().Contain("invalid characters");
        vehicle.LicensePlate.Should().Be(originalPlate);
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void UpdateHorsePower_Should_UpdateHpAndRaiseEvent_WhenNewHpIsValid()
    {
        // Arrange
        var vehicle = new VehicleBuilder().Build();
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
        var vehicle = new VehicleBuilder().Build();
        var invalidHpInput = -10;
        var originalHp = vehicle.HorsePower;
        vehicle.ClearDomainEvents();

        // Act
        var result = vehicle.UpdateHorsePower(invalidHpInput);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Message.Should().Contain("positive value");
        vehicle.HorsePower.Should().Be(originalHp);
        vehicle.GetDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void Create_Should_ReturnError_WhenLicensePlateIsTooLong()
    {
        // Arrange
        var builder = new VehicleBuilder().WithLicensePlate("THIS IS A VERY LONG LICENSE PLATE THAT EXCEEDS THE LIMIT");

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("LicensePlate");
    }

    [Fact]
    public void Create_Should_ReturnError_WhenHorsePowerIsZero()
    {
        // Arrange
        var builder = new VehicleBuilder().WithHorsePower(0);

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("HorsePower");
    }

    [Fact]
    public void Create_Should_ReturnError_WhenMileageIsZero()
    {
        // Arrange
        var builder = new VehicleBuilder().WithMileage(0, EMileageUnit.Kilometers);

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("Mileage");
    }

    [Fact]
    public void Create_Should_ReturnError_WhenProductionYearIsFuture()
    {
        // Arrange
        var futureYear = DateTime.UtcNow.AddYears(2).Year.ToString(CultureInfo.InvariantCulture);
        var builder = new VehicleBuilder().WithProductionYear(futureYear);

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("Year");
    }

    [Fact]
    public void Create_Should_ReturnError_WhenEngineCapacityIsZero()
    {
        // Arrange
        var builder = new VehicleBuilder().WithEngineCapacity(0);

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("Engine capacity");
    }
}

namespace eMechanic.Application.Tests.Vehicle.Features.Create;

using eMechanic.Application.Vehicle.Features.Create;
using eMechanic.Domain.Vehicle.Enums;
using FluentValidation.TestHelper;

public class CreateVehicleCommandValidatorTests
{
    private readonly CreateVehicleCommandValidator _validator = new();

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateVehicleCommand(
            "VIN123456789ABCDE",
            "Test Manufacturer",
            "Test Model",
            "2023",
            1.6m,
            200,
            EMileageUnit.Miles,
            "PZ1W924",
            124,
            EFuelType.Gasoline,
            EBodyType.Sedan,
            EVehicleType.Passenger);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("SHORTVIN")]
    [InlineData("VIN_WITH_INVALID_CHARS_")]
    [InlineData("VIN123456789ABCDEFGH")]
    public void Should_HaveError_WhenVinIsInvalid(string invalidVin)
    {
        // Arrange
        var command = new CreateVehicleCommand(
            invalidVin,
            "Test Manufacturer",
            "Test Model",
            "2023",
            1.6m,
            200,
            EMileageUnit.Miles,
            "PZ1W924",
            124,
            EFuelType.Gasoline,
            EBodyType.Sedan,
            EVehicleType.Passenger);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Vin);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_HaveError_WhenManufacturerIsEmpty(string? invalidManufacturer)
    {
         // Arrange
        var command = new CreateVehicleCommand(
            "VIN123456789ABCDE",
            invalidManufacturer!,
            "Test Model",
            "2023",
            1.6m,
            200,
            EMileageUnit.Miles,
            "PZ1W924",
            124,
            EFuelType.Gasoline,
            EBodyType.Sedan,
            EVehicleType.Passenger);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Manufacturer);
    }

    [Fact]
    public void Should_HaveError_WhenEngineCapacityIsNegative()
    {
         // Arrange
        var command = new CreateVehicleCommand(
            "VIN123456789ABCDE",
            "Test Manufacturer",
            "Test Model",
            "2023",
            -1.0m,
            200,
            EMileageUnit.Miles,
            "PZ1W924",
            124,
            EFuelType.Gasoline,
            EBodyType.Sedan,
            EVehicleType.Passenger);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EngineCapacity);
    }

    [Fact]
    public void Should_HaveError_WhenFuelTypeIsNone()
    {
         // Arrange
        var command = new CreateVehicleCommand(
            "VIN123456789ABCDE",
            "Test Manufacturer",
            "Test Model",
            "2023",
            1.6m,
            200,
            EMileageUnit.Miles,
            "PZ1W924",
            124,
            EFuelType.None,
            EBodyType.Sedan,
            EVehicleType.Passenger);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FuelType);
    }

     [Fact]
    public void Should_HaveError_WhenBodyTypeIsNone()
    {
         // Arrange
        var command = new CreateVehicleCommand(
            "VIN123456789ABCDE",
            "Test Manufacturer",
            "Test Model",
            "2023",
            1.6m,
            200,
            EMileageUnit.Miles,
            "PZ1W924",
            124,
            EFuelType.Gasoline,
            EBodyType.None,
            EVehicleType.Passenger);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BodyType);
    }

    [Fact]
    public void Should_NotHaveError_WhenVehicleTypeIsMotorcycleAndBodyTypeIsNone()
    {
        // Arrange
        var command = new CreateVehicleCommand(
            "VIN123456789ABCDE",
            "Test Manufacturer",
            "Test Model",
            "2023",
            1.6m,
            200,
            EMileageUnit.Miles,
            "PZ1W924",
            124,
            EFuelType.Gasoline,
            EBodyType.None,
            EVehicleType.Motorcycle);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BodyType);
        result.ShouldNotHaveValidationErrorFor(x => x.VehicleType);
    }

    [Theory]
    [InlineData("BAD PLATE!")]
    [InlineData("WAYTOOLONGPLATE123456789")]
    public void Should_HaveError_WhenLicensePlateIsInvalid(string invalidPlate)
    {
        // Arrange
        var command = new CreateVehicleCommand(
            "VIN123456789ABCDE",
            "Test Manufacturer",
            "Test Model",
            "2023",
            1.6m,
            200,
            EMileageUnit.Miles,
            invalidPlate,
            150,
            EFuelType.Gasoline, EBodyType.Sedan, EVehicleType.Passenger);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LicensePlate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    [InlineData(20000)]
    public void Should_HaveError_WhenHorsePowerIsInvalid(int invalidHp)
    {
        // Arrange
        var command = new CreateVehicleCommand(
            "VIN123456789ABCDE",
            "Test Manufacturer",
            "Test Model",
            "2023",
            1.6m,
            200,
            EMileageUnit.Miles,
            "PO 12345",
            invalidHp,
            EFuelType.Gasoline, EBodyType.Sedan, EVehicleType.Passenger);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.HorsePower);
    }

    [Fact]
    public void Should_NotHaveError_WhenLicensePlateAndHpAreValid()
    {
        // Arrange
        var command = new CreateVehicleCommand(
            "VIN123456789ABCDE",
            "Test Manufacturer",
            "Test Model",
            "2023",
            1.6m,
            200,
            EMileageUnit.Miles,
            "PO 12345",
            150,
            EFuelType.Gasoline,
            EBodyType.Sedan,
            EVehicleType.Passenger);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.LicensePlate);
        result.ShouldNotHaveValidationErrorFor(x => x.HorsePower);
    }
}

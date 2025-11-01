namespace eMechanic.Application.Tests.Vehicle.Create;

using Application.Vehicle.Create;
using Domain.Vehicle.Enums;
using FluentValidation.TestHelper;

public class CreateVehicleCommandValidatorTests
{
    private readonly CreateVehicleCommandValidator _validator = new();

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateVehicleCommand(
            "VIN123456789ABCDE", "Test Manufacturer", "Test Model", "2023",
            1.6m,  200, EMileageUnit.Miles, EFuelType.Gasoline, EBodyType.Sedan, EVehicleType.Passenger);

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
            invalidVin, "Test Manufacturer", "Test Model", "2023",
            1.6m,  200, EMileageUnit.Miles, EFuelType.Gasoline, EBodyType.Sedan, EVehicleType.Passenger);

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
            "VIN123456789ABCDE", invalidManufacturer!, "Test Model", "2023",
            1.6m,  200, EMileageUnit.Miles, EFuelType.Gasoline, EBodyType.Sedan, EVehicleType.Passenger);

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
            "VIN123456789ABCDE", "Test Manufacturer", "Test Model", "2023",
            -1.0m,  200, EMileageUnit.Miles, EFuelType.Gasoline, EBodyType.Sedan, EVehicleType.Passenger);

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
            "VIN123456789ABCDE", "Test Manufacturer", "Test Model", "2023",
            1.6m,  200, EMileageUnit.Miles, EFuelType.None, EBodyType.Sedan, EVehicleType.Passenger);

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
            "VIN123456789ABCDE", "Test Manufacturer", "Test Model", "2023",
            1.6m,  200, EMileageUnit.Miles, EFuelType.Gasoline, EBodyType.None, EVehicleType.Passenger);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BodyType);
    }

     [Fact]
    public void Should_HaveError_WhenVehicleTypeIsNone()
    {
         // Arrange
        var command = new CreateVehicleCommand(
            "VIN123456789ABCDE", "Test Manufacturer", "Test Model", "2023",
            1.6m,  200, EMileageUnit.Miles, EFuelType.Gasoline, EBodyType.Sedan, EVehicleType.None);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.VehicleType);
    }
}

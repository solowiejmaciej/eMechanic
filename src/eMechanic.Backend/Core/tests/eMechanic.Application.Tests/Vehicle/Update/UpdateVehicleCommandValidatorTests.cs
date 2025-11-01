namespace eMechanic.Application.Tests.Vehicle.Update;

using Application.Vehicle.Update;
using Domain.Vehicle.Enums;
using FluentValidation.TestHelper;

public class UpdateVehicleCommandValidatorTests
{
    private readonly UpdateVehicleCommandValidator _validator = new();

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValid()
    {
        // Arrange
        var command = new UpdateVehicleCommand(
            Guid.NewGuid(), "VIN123456789ABCDE", "Test Manufacturer", "Test Model", "2023",
            1.6m,  200, EMileageUnit.Kilometers, EFuelType.Gasoline, EBodyType.Sedan, EVehicleType.Passenger);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenIdIsEmpty()
    {
        // Arrange
        var command = new UpdateVehicleCommand(
            Guid.Empty, "VIN123456789ABCDE", "Test Manufacturer", "Test Model", "2023",
            1.6m,  200, EMileageUnit.Kilometers,EFuelType.Gasoline, EBodyType.Sedan, EVehicleType.Passenger);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("SHORTVIN")]
    [InlineData("VIN_WITH_INVALID_CHARS_")]
    [InlineData("VIN123456789ABCDEFGH")]
    public void Should_HaveError_WhenVinIsInvalid(string invalidVin)
    {
        var command = new UpdateVehicleCommand(
            Guid.NewGuid(), invalidVin, "Test Manufacturer", "Test Model", "2023",
            1.6m,  200, EMileageUnit.Kilometers, EFuelType.Gasoline, EBodyType.Sedan, EVehicleType.Passenger);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Vin);
    }

    [Fact]
    public void Should_HaveError_WhenMileageIsNegative()
    {
        // Arrange
        var command = new UpdateVehicleCommand(
            Guid.NewGuid(), "V1N123456789ABCDE", "Test Manufacturer", "Test Model", "2023",
            1.6m, -100, EMileageUnit.Kilometers, EFuelType.Gasoline, EBodyType.Sedan, EVehicleType.Passenger);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MillageValue);
    }

    [Fact]
    public void Should_HaveError_WhenMileageIsNull()
    {
        // Arrange
        var command = new UpdateVehicleCommand(
            Guid.NewGuid(), "VIN123456789ABCDE", "Test Manufacturer", "Test Model", "2023",
            1.6m, int.MinValue, EMileageUnit.Kilometers, EFuelType.Gasoline, EBodyType.Sedan, EVehicleType.Passenger);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MillageValue);
    }

    [Fact]
    public void Should_HaveError_WhenMileageUnitIsNone()
    {
        // Arrange
        var command = new UpdateVehicleCommand(
            Guid.NewGuid(), "VIN123456789ABCDE", "Test Manufacturer", "Test Model", "2023",
            1.6m, 10000, EMileageUnit.None, EFuelType.Gasoline, EBodyType.Sedan, EVehicleType.Passenger);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MillageUnit);
    }
}

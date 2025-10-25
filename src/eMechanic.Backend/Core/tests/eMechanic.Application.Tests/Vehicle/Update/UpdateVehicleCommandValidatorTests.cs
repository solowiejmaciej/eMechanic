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
            1.6m, EFuelType.Gasoline, EBodyType.Sedan, EVehicleType.Passenger);

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
            1.6m, EFuelType.Gasoline, EBodyType.Sedan, EVehicleType.Passenger);

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
            1.6m, EFuelType.Gasoline, EBodyType.Sedan, EVehicleType.Passenger);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Vin);
    }

    // ... (dodać resztę analogicznych testów dla pozostałych pól) ...
}

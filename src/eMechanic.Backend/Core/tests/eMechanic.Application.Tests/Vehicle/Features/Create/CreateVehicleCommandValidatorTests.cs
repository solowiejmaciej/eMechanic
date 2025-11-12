namespace eMechanic.Application.Tests.Vehicle.Features.Create;

using Application.Tests.Builders;
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
        var command = new CreateVehicleCommandBuilder().Build();

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
        var command = new CreateVehicleCommandBuilder().WithVin(invalidVin).Build();

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
        var command = new CreateVehicleCommandBuilder().WithManufacturer(invalidManufacturer!).Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Manufacturer);
    }

    [Fact]
    public void Should_HaveError_WhenEngineCapacityIsNegative()
    {
        // Arrange
        var command = new CreateVehicleCommandBuilder().WithEngineCapacity(-1.0m).Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EngineCapacity);
    }

    [Fact]
    public void Should_HaveError_WhenFuelTypeIsNone()
    {
        // Arrange
        var command = new CreateVehicleCommandBuilder().WithFuelType(EFuelType.None).Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FuelType);
    }

    [Fact]
    public void Should_HaveError_WhenBodyTypeIsNone()
    {
        // Arrange
        var command = new CreateVehicleCommandBuilder().WithBodyType(EBodyType.None).Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BodyType);
    }

    [Fact]
    public void Should_NotHaveError_WhenVehicleTypeIsMotorcycleAndBodyTypeIsNone()
    {
        // Arrange
        var command = new CreateVehicleCommandBuilder()
            .WithVehicleType(EVehicleType.Motorcycle)
            .WithBodyType(EBodyType.None)
            .Build();

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
        var command = new CreateVehicleCommandBuilder().WithLicensePlate(invalidPlate).Build();

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
        var command = new CreateVehicleCommandBuilder().WithHorsePower(invalidHp).Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.HorsePower);
    }

    [Fact]
    public void Should_NotHaveError_WhenLicensePlateAndHpAreValid()
    {
        // Arrange
        var command = new CreateVehicleCommandBuilder()
            .WithLicensePlate("PO 12345")
            .WithHorsePower(150)
            .Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.LicensePlate);
        result.ShouldNotHaveValidationErrorFor(x => x.HorsePower);
    }
}

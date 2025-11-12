namespace eMechanic.Application.Tests.Vehicle.Features.Update;

using System;
using Application.Tests.Builders;
using eMechanic.Application.Vehicle.Features.Update;
using eMechanic.Domain.Vehicle.Enums;
using FluentValidation.TestHelper;

public class UpdateVehicleCommandValidatorTests
{
    private readonly UpdateVehicleCommandValidator _validator = new();

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValid()
    {
        // Arrange
        var command = new UpdateVehicleCommandBuilder().Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenIdIsEmpty()
    {
        // Arrange
        var command = new UpdateVehicleCommandBuilder().WithId(Guid.Empty).Build();

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
        var command = new UpdateVehicleCommandBuilder().WithVin(invalidVin).Build();
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Vin);
    }

    [Fact]
    public void Should_HaveError_WhenMileageIsNegative()
    {
        // Arrange
        var command = new UpdateVehicleCommandBuilder().WithMileage(-100).Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MillageValue);
    }

    [Fact]
    public void Should_HaveError_WhenMileageUnitIsNone()
    {
        // Arrange
        var command = new UpdateVehicleCommandBuilder().WithMileageUnit(EMileageUnit.None).Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MillageUnit);
    }

    [Theory]
    [InlineData("BAD PLATE!")]
    [InlineData("WAYTOOLONGPLATE123456789")]
    public void Should_HaveError_WhenLicensePlateIsInvalid(string invalidPlate)
    {
        // Arrange
        var command = new UpdateVehicleCommandBuilder().WithLicensePlate(invalidPlate).Build();

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
        var command = new UpdateVehicleCommandBuilder().WithHorsePower(invalidHp).Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.HorsePower);
    }

    [Fact]
    public void Should_NotHaveError_WhenLicensePlateAndHpAreValid()
    {
        // Arrange
        var command = new UpdateVehicleCommandBuilder().WithLicensePlate("PO 12345").WithHorsePower(150).Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.LicensePlate);
        result.ShouldNotHaveValidationErrorFor(x => x.HorsePower);
    }
}

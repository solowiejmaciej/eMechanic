namespace eMechanic.Application.Tests.Vehicle.Features.Delete;

using System;
using Application.Tests.Builders;
using eMechanic.Application.Vehicle.Features.Delete;
using FluentValidation.TestHelper;

public class DeleteVehicleCommandValidatorTests
{
    private readonly DeleteVehicleCommandValidator _validator = new();

    [Fact]
    public void Should_NotHaveError_WhenIdIsValid()
    {
        // Arrange
        var command = new DeleteVehicleCommandBuilder().Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenIdIsEmpty()
    {
        // Arrange
        var command = new DeleteVehicleCommandBuilder().WithId(Guid.Empty).Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}

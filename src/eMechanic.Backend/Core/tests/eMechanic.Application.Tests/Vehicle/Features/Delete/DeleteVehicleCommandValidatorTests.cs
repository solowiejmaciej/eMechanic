namespace eMechanic.Application.Tests.Vehicle.Delete;

using Application.Vehicle.Features.Delete;
using FluentValidation.TestHelper;

public class DeleteVehicleCommandValidatorTests
{
    private readonly DeleteVehicleCommandValidator _validator = new();

    [Fact]
    public void Should_NotHaveError_WhenIdIsValid()
    {
        // Arrange
        var command = new DeleteVehicleCommand(Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenIdIsEmpty()
    {
        // Arrange
        var command = new DeleteVehicleCommand(Guid.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}

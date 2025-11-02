namespace eMechanic.Application.Tests.Vehicle.Get.GetById;

using Application.Vehicle.Features.Get.ById;
using FluentValidation.TestHelper;

public class GetVehicleByIdQueryValidatorTests
{
    private readonly GetVehicleByIdQueryValidator _validator = new();

    [Fact]
    public void Should_NotHaveError_WhenIdIsValid()
    {
        // Arrange
        var query = new GetVehicleByIdQuery(Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenIdIsEmpty()
    {
        // Arrange
        var query = new GetVehicleByIdQuery(Guid.Empty);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}

namespace eMechanic.Application.Tests.Vehicle.Features.Get.GetById;

using System;
using Application.Tests.Builders;
using eMechanic.Application.Vehicle.Features.Get.ById;
using FluentValidation.TestHelper;

public class GetVehicleByIdQueryValidatorTests
{
    private readonly GetVehicleByIdQueryValidator _validator = new();

    [Fact]
    public void Should_NotHaveError_WhenIdIsValid()
    {
        // Arrange
        var query = new GetVehicleByIdQueryBuilder().Build();

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenIdIsEmpty()
    {
        // Arrange
        var query = new GetVehicleByIdQueryBuilder().WithId(Guid.Empty).Build();

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}

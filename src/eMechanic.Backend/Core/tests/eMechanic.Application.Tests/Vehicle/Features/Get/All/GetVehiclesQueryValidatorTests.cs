namespace eMechanic.Application.Tests.Vehicle.Features.Get.All;

using Application.Tests.Builders;
using eMechanic.Application.Vehicle.Features.Get.All;
using eMechanic.Common.Result;
using FluentValidation.TestHelper;

public class GetVehiclesQueryValidatorTests
{
    private readonly GetVehiclesQueryValidator _validator = new();

    [Fact]
    public void Should_NotHaveError_WhenParametersAreValid()
    {
        // Arrange
        var parameters = new PaginationParametersBuilder().Build();
        var query = new GetVehiclesQuery(parameters);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenPageNumberIsInvalid()
    {
        // Arrange
        var parameters = new PaginationParametersBuilder().WithPageNumber(0).Build();
        var query = new GetVehiclesQuery(parameters);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PaginationParameters.PageNumber);
    }

    [Fact]
    public void Should_HaveError_WhenPageSizeIsInvalid()
    {
        // Arrange
        var parameters = new PaginationParametersBuilder().WithPageSize(0).Build();
        var query = new GetVehiclesQuery(parameters);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PaginationParameters.PageSize);
    }
}

namespace eMechanic.Application.Tests.Vehicle.Get.All;


using Application.Vehicle.Get.All;
using Common.Result;
using FluentValidation.TestHelper;

public class GetVehiclesQueryValidatorTests
{
    private readonly GetVehiclesQueryValidator _validator = new();

    [Fact]
    public void Should_NotHaveError_WhenParametersAreValid()
    {
        // Arrange
        var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10 };
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
        var parameters = new PaginationParameters { PageNumber = 0, PageSize = 10 };
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
        var parameters = new PaginationParameters { PageNumber = 1, PageSize = 0 };
        var query = new GetVehiclesQuery(parameters);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PaginationParameters.PageSize);
    }
}

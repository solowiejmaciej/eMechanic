namespace eMechanic.Application.Tests.Vehicle.Features.Get.Timeline;

using eMechanic.Application.Vehicle.Features.Get.Timeline;
using eMechanic.Common.Result;
using FluentValidation.TestHelper;

public class GetVehicleTimelineByVehicleIdQueryValidatorTests
{
    private readonly GetVehicleTimelineByVehicleIdQueryValidator _validator = new();

    [Fact]
    public void Should_NotHaveError_WhenQueryIsValid()
    {
        // Arrange
        var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10 };
        var query = new GetVehicleTimelineByVehicleIdQuery(Guid.NewGuid(), parameters);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenVehicleIdIsEmpty()
    {
        // Arrange
        var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10 };
        var query = new GetVehicleTimelineByVehicleIdQuery(Guid.Empty, parameters);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.VehicleId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_HaveError_WhenPageNumberIsInvalid(int invalidPageNumber)
    {
        // Arrange
        var parameters = new PaginationParameters { PageNumber = invalidPageNumber, PageSize = 10 };
        var query = new GetVehicleTimelineByVehicleIdQuery(Guid.NewGuid(), parameters);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PaginationParameters.PageNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_HaveError_WhenPageSizeIsInvalid(int invalidPageSize)
    {
        // Arrange
        var parameters = new PaginationParameters { PageNumber = 1, PageSize = invalidPageSize };
        var query = new GetVehicleTimelineByVehicleIdQuery(Guid.NewGuid(), parameters);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PaginationParameters.PageSize);
    }
}

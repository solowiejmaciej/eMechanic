namespace eMechanic.Application.Tests.Vehicle.Features.Get.Timeline;

using System;
using Application.Tests.Builders;
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
        var query = new GetVehicleTimelineByVehicleIdQueryBuilder().Build();

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenVehicleIdIsEmpty()
    {
        // Arrange
        var query = new GetVehicleTimelineByVehicleIdQueryBuilder().WithVehicleId(Guid.Empty).Build();

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
        var parameters = new PaginationParametersBuilder().WithPageNumber(invalidPageNumber).Build();
        var query = new GetVehicleTimelineByVehicleIdQueryBuilder().WithPaginationParameters(parameters).Build();

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
        var parameters = new PaginationParametersBuilder().WithPageSize(invalidPageSize).Build();
        var query = new GetVehicleTimelineByVehicleIdQueryBuilder().WithPaginationParameters(parameters).Build();

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PaginationParameters.PageSize);
    }
}

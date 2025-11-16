namespace eMechanic.Application.Tests.VehicleDocument.Features.Get.All;

using eMechanic.Application.Tests.Builders;
using eMechanic.Application.VehicleDocument.Features.Get.All;
using FluentValidation.TestHelper;

public class GetVehicleDocumentsQueryValidatorTests
{
    private readonly GetVehicleDocumentsQueryValidator _validator = new();

    [Fact]
    public void Should_NotHaveError_WhenQueryIsValid()
    {
        var query = new GetVehicleDocumentsQueryBuilder().Build();
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenVehicleIdIsEmpty()
    {
        var query = new GetVehicleDocumentsQueryBuilder().WithVehicleId(Guid.Empty).Build();
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.VehicleId);
    }

    [Fact]
    public void Should_HaveError_WhenPageNumberIsInvalid()
    {
        var query = new GetVehicleDocumentsQueryBuilder().WithPagination(0, 10).Build();
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.PaginationParameters.PageNumber);
    }

    [Fact]
    public void Should_HaveError_WhenPageSizeIsInvalid()
    {
        var query = new GetVehicleDocumentsQueryBuilder().WithPagination(1, 0).Build();
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.PaginationParameters.PageSize);
    }
}

namespace eMechanic.Application.Tests.VehicleDocument.Features.Get.ById;

using eMechanic.Application.Tests.Builders;
using eMechanic.Application.VehicleDocument.Features.Get.ById;
using FluentValidation.TestHelper;

public class GetVehicleDocumentFileQueryValidatorTests
{
    private readonly GetVehicleDocumentFileQueryValidator _validator = new();

    [Fact]
    public void Should_NotHaveError_WhenQueryIsValid()
    {
        var query = new GetVehicleDocumentFileQueryBuilder().Build();
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenVehicleIdIsEmpty()
    {
        var query = new GetVehicleDocumentFileQueryBuilder().WithVehicleId(Guid.Empty).Build();
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.VehicleId);
    }

    [Fact]
    public void Should_HaveError_WhenDocumentIdIsEmpty()
    {
        var query = new GetVehicleDocumentFileQueryBuilder().WithDocumentId(Guid.Empty).Build();
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.DocumentId);
    }
}

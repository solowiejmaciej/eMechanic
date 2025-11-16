namespace eMechanic.Application.Tests.VehicleDocument.Features;

using Application.VehicleDocument.Features.Delete;
using Builders;
using FluentValidation.TestHelper;

public class DeleteVehicleDocumentCommandValidatorTests
{
    private readonly DeleteVehicleDocumentCommandValidator _validator = new();

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValid()
    {
        var command = new DeleteVehicleDocumentCommandBuilder().Build();
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenVehicleIdIsEmpty()
    {
        var command = new DeleteVehicleDocumentCommandBuilder().WithVehicleId(Guid.Empty).Build();
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.VehicleId);
    }

    [Fact]
    public void Should_HaveError_WhenDocumentIdIsEmpty()
    {
        var command = new DeleteVehicleDocumentCommandBuilder().WithDocumentId(Guid.Empty).Build();
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DocumentId);
    }
}

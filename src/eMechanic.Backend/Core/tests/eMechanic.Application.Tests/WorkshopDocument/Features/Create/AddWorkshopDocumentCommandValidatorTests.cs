namespace eMechanic.Application.Tests.WorkshopDocument.Features.Create;

using Application.Workshop.Document.Features.Create;
using Builders.WorkshopDocument;
using Domain.Workshop.Documents.Enums;
using eMechanic.Application.Tests.Builders;
using eMechanic.Domain.Vehicle.Documents.Enums;
using FluentValidation.TestHelper;

public class AddWorkshopDocumentCommandValidatorTests
{
    private readonly AddWorkshopDocumentCommandValidator _validator = new();

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValidPdf()
    {
        // Arrange
        var command = new AddWorkshopDocumentCommandBuilder()
            .Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValidImage()
    {
        // Arrange
        var command = new AddWorkshopDocumentCommandBuilder()
            .WithDocumentType(EWorkshopDocumentType.GalleryImage)
            .Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenDocumentTypeIsNone()
    {
        var command = new AddWorkshopDocumentCommandBuilder().WithDocumentType(EWorkshopDocumentType.None).Build();
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DocumentType);
    }
}

namespace eMechanic.Application.Tests.VehicleDocument.Features.Create;

using eMechanic.Application.Tests.Builders;
using eMechanic.Application.VehicleDocument.Features.Create;
using eMechanic.Domain.VehicleDocument.Enums;
using FluentValidation.TestHelper;

public class AddVehicleDocumentCommandValidatorTests
{
    private readonly AddVehicleDocumentCommandValidator _validator = new();

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValidPdf()
    {
        // Arrange
        var command = new AddVehicleDocumentCommandBuilder()
            .WithFile("test.pdf", "application/pdf", 1024 * 1024) // 1MB
            .WithDocumentType(EVehicleDocumentType.Invoice)
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
        var command = new AddVehicleDocumentCommandBuilder()
            .WithFile("test.jpg", "image/jpeg", 1024 * 1024) // 1MB
            .WithDocumentType(EVehicleDocumentType.Photo)
            .Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenVehicleIdIsEmpty()
    {
        var command = new AddVehicleDocumentCommandBuilder().WithVehicleId(Guid.Empty).Build();
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.VehicleId);
    }

    [Fact]
    public void Should_HaveError_WhenDocumentTypeIsNone()
    {
        var command = new AddVehicleDocumentCommandBuilder().WithDocumentType(EVehicleDocumentType.None).Build();
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DocumentType);
    }

    [Fact]
    public void Should_HaveError_WhenFileIsTooLarge()
    {
        long tooLarge = (10 * 1024 * 1024) + 1;
        var command = new AddVehicleDocumentCommandBuilder()
            .WithFile("large.pdf", "application/pdf", tooLarge)
            .Build();

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.File.Length);
    }

    [Theory]
    [InlineData("text/plain")]
    [InlineData("application/msword")]
    [InlineData("video/mp4")]
    public void Should_HaveError_WhenContentTypeIsInvalid(string invalidType)
    {
        var command = new AddVehicleDocumentCommandBuilder()
            .WithFile("invalid.txt", invalidType, 1024)
            .Build();

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.File.ContentType);
    }
}

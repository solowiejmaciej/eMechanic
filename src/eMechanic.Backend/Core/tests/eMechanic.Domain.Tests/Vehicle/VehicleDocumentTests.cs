namespace eMechanic.Domain.Tests.Vehicle;

using eMechanic.Common.Result;
using eMechanic.Domain.Tests.Builders;
using eMechanic.Domain.VehicleDocument.DomainEvents;
using eMechanic.Domain.VehicleDocument.Enums;
using FluentAssertions;

public class VehicleDocumentTests
{
    [Fact]
    public void Create_Should_ReturnSuccessAndRaiseEvent_WhenDataIsValid()
    {
        // Arrange
        var builder = new VehicleDocumentBuilder();

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeFalse();
        var document = result.Value;

        document.Should().NotBeNull();
        document.Id.Should().NotBeEmpty();
        document.VehicleId.Should().NotBeEmpty();
        document.FullPath.Should().NotBeNullOrWhiteSpace();
        document.OriginalFileName.Should().NotBeNullOrWhiteSpace();
        document.ContentType.Should().NotBeNullOrWhiteSpace();
        document.DocumentType.Should().NotBe(EVehicleDocumentType.None);

        document.GetDomainEvents().Should().HaveCount(1);
        document.GetDomainEvents().First().Should().BeOfType<VehicleDocumentAddedDomainEvent>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ReturnValidationError_WhenFullPathIsInvalid(string invalidPath)
    {
        // Arrange
        var builder = new VehicleDocumentBuilder().WithFullPath(invalidPath);

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("File path can't be empty");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ReturnValidationError_WhenOriginalFileNameIsInvalid(string invalidName)
    {
        // Arrange
        var builder = new VehicleDocumentBuilder().WithOriginalFileName(invalidName);

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("Name can't be empty");
    }

    [Fact]
    public void Create_Should_ReturnValidationError_WhenIdIsEmpty()
    {
        // Arrange
        var builder = new VehicleDocumentBuilder().WithId(Guid.Empty);

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Message.Should().Contain("Id can't be empty");
    }

    [Fact]
    public void Create_Should_ReturnValidationError_WhenVehicleIdIsEmpty()
    {
        // Arrange
        var builder = new VehicleDocumentBuilder().WithVehicleId(Guid.Empty);

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Message.Should().Contain("VehicleId can't be empty");
    }

    [Fact]
    public void Create_Should_ReturnValidationError_WhenDocumentTypeIsNone()
    {
        // Arrange
        var builder = new VehicleDocumentBuilder().WithDocumentType(EVehicleDocumentType.None);

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Message.Should().Contain("Vehicle document type can't be None");
    }
}

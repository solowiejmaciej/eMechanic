namespace eMechanic.Domain.Tests.Workshop.Documents;

using Builders;
using Common.Result;
using Domain.Workshop.Documents.Enums;
using FluentAssertions;
using Xunit;

public class WorkshopDocumentTests
{
    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var builder = new WorkshopDocumentBuilder();

        // Act
        var result = builder.BuildResult();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public void Create_ShouldReturnError_WhenIdIsEmpty()
    {
        // Arrange
        var builder = new WorkshopDocumentBuilder().WithId(Guid.Empty);

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("Id cannot be empty");
    }

    [Fact]
    public void Create_ShouldReturnError_WhenWorkshopIdIsEmpty()
    {
        // Arrange
        var builder = new WorkshopDocumentBuilder().WithWorkshopId(Guid.Empty);

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("WorkshopId cannot be empty");
    }

    [Fact]
    public void Create_ShouldReturnError_WhenDocumentTypeIsNone()
    {
        // Arrange
        var builder = new WorkshopDocumentBuilder().WithDocumentType(EWorkshopDocumentType.None);

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("Invalid document type");
    }
}

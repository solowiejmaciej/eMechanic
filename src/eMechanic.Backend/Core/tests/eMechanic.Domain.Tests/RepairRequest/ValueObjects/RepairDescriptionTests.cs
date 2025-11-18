namespace eMechanic.Domain.Tests.RepairRequest.ValueObjects;

using Common.Result;
using Domain.RepairRequest.ValueObjects;
using FluentAssertions;
using Xunit;

public class RepairDescriptionTests
{
    [Fact]
    public void Create_Should_ReturnSuccess_WhenDescriptionIsValid()
    {
        // Arrange
        var validDescription = "Problem with the engine making weird noise.";

        // Act
        var result = RepairDescription.Create(validDescription);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value!.Value.Should().Be(validDescription);
    }

    [Fact]
    public void Create_Should_TrimWhitespace()
    {
        // Arrange
        var description = "   Trim me please   ";

        // Act
        var result = RepairDescription.Create(description);

        // Assert
        result.Value!.Value.Should().Be("Trim me please");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ReturnError_WhenDescriptionIsEmpty(string? invalidDesc)
    {
        // Act
        var result = RepairDescription.Create(invalidDesc);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
    }

    [Fact]
    public void Create_Should_ReturnError_WhenDescriptionIsTooShort()
    {
        // Arrange
        var shortDesc = "Too short";

        // Act
        var result = RepairDescription.Create(shortDesc);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Message.Should().Contain("at least 10 characters");
    }

    [Fact]
    public void Create_Should_ReturnError_WhenDescriptionIsTooLong()
    {
        // Arrange
        var longDesc = new string('a', 2001);

        // Act
        var result = RepairDescription.Create(longDesc);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Message.Should().Contain("cannot exceed 2000 characters");
    }
}

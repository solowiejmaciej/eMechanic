namespace eMechanic.Domain.Tests.RepairRequest.ValueObjects;

using Common.Result;
using Domain.RepairRequest.ValueObjects;
using FluentAssertions;
using Xunit;

public class RepairDiagnosisTests
{
    [Fact]
    public void Create_Should_ReturnSuccess_WhenDiagnosisIsValid()
    {
        // Act
        var result = RepairDiagnosis.Create("Faulty spark plugs.");

        // Assert
        result.HasError().Should().BeFalse();
        result.Value!.Value.Should().Be("Faulty spark plugs.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ReturnError_WhenDiagnosisIsEmpty(string? invalidInput)
    {
        // Act
        var result = RepairDiagnosis.Create(invalidInput);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
    }

    [Fact]
    public void Create_Should_ReturnError_WhenDiagnosisIsTooLong()
    {
        // Arrange
        var longText = new string('x', 4001);

        // Act
        var result = RepairDiagnosis.Create(longText);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Message.Should().Contain("cannot exceed 4000 characters");
    }
}

namespace eMechanic.Domain.Tests.Vehicle.ValueObjects;

using Common.Result;
using Domain.Vehicle.ValueObjects;
using FluentAssertions;

public class ProductionYearTests
{
    private const string VALID_YEAR_STRING = "2023";
    private const int MIN_YEAR = 1886;
    private static readonly int MAX_YEAR = DateTime.UtcNow.Year + 1;
    private const string YEAR_BELOW_MIN = "1885";
    private const string YEAR_ABOVE_MAX = "2101";
    private const string INVALID_YEAR_FORMAT = "twenty twenty three";

    [Fact]
    public void Create_Should_ReturnSuccess_WhenYearStringIsValid()
    {
        // Act
        var result = ProductionYear.Create(VALID_YEAR_STRING);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(VALID_YEAR_STRING);
    }

    [Fact]
    public void Create_Should_ReturnSuccess_WhenYearStringHasSpaces()
    {
        // Act
        var result = ProductionYear.Create(" 2023 ");

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be("2023"); // Oczekujemy przyciętej wartości
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ReturnError_WhenYearIsNullOrEmptyOrWhitespace(string? invalidInput)
    {
        // Act
        var result = ProductionYear.Create(invalidInput!); // Test dla wartości null

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("Year cannot be null or empty");
    }

    [Fact]
    public void Create_Should_ReturnError_WhenYearIsNotAnInteger()
    {
        // Act
        var result = ProductionYear.Create(INVALID_YEAR_FORMAT);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("Year must be a valid integer");
    }

    [Fact]
    public void Create_Should_ReturnError_WhenYearIsBelowMinimum()
    {
        // Act
        var result = ProductionYear.Create(YEAR_BELOW_MIN);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain($"must be between {MIN_YEAR} and {MAX_YEAR}");
    }

    [Fact]
    public void Create_Should_ReturnError_WhenYearIsAboveMaximum()
    {
        // Act
        var result = ProductionYear.Create(YEAR_ABOVE_MAX);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain($"must be between {MIN_YEAR} and {MAX_YEAR}");
    }

    [Fact]
    public void ImplicitConversion_ToString_Should_ReturnValue()
    {
        // Arrange
        var result = ProductionYear.Create(VALID_YEAR_STRING);
        result.HasError().Should().BeFalse();
        var year = result.Value!;

        // Act
        string yearString = year;

        // Assert
        yearString.Should().Be(VALID_YEAR_STRING);
    }

    [Fact]
    public void ToString_Should_ReturnValue()
    {
        // Arrange
        var result = ProductionYear.Create(VALID_YEAR_STRING);
        result.HasError().Should().BeFalse();
        var year = result.Value!;

        // Act
        var yearString = year.ToString();

        // Assert
        yearString.Should().Be(VALID_YEAR_STRING);
    }

    [Fact]
    public void Equality_Should_BeBasedOnValue()
    {
        // Arrange
        var y1Result = ProductionYear.Create("2020");
        var y2Result = ProductionYear.Create(" 2020 ");
        var y3Result = ProductionYear.Create("2021");

        y1Result.HasError().Should().BeFalse();
        y2Result.HasError().Should().BeFalse();
        y3Result.HasError().Should().BeFalse();

        var y1 = y1Result.Value!;
        var y2 = y2Result.Value!;
        var y3 = y3Result.Value!;

        // Assert
        y1.Should().Be(y2);
        y1.Should().NotBe(y3);
        (y1 == y2).Should().BeTrue();
        (y1 != y3).Should().BeTrue();
    }
}

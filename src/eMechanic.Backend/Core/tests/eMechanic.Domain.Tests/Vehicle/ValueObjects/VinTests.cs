namespace eMechanic.Domain.Tests.Vehicle.ValueObjects;

using Domain.Vehicle.ValueObjects;
using Common.Result;
using FluentAssertions;

public class VinTests
{
    private const string VALID_VIN = "V1N123456789ABCDE";
    private const string VALID_VIN_LOWERCASE = "v1n123456789abcde";
    private const string INVALID_VIN_SHORT = "VIN123";
    private const string INVALID_VIN_LONG = "VIN123456789ABCDEFGH";
    private const string INVALID_VIN_CHARS = "VIN123456!@#$%^&*";

    [Fact]
    public void Create_Should_ReturnSuccess_WhenVinIsValid()
    {
        // Act
        var result = Vin.Create(VALID_VIN);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(VALID_VIN);
    }

    [Fact]
    public void Create_Should_ReturnSuccessAndNormalize_WhenVinIsValidLowercase()
    {
        // Act
        var result = Vin.Create(VALID_VIN_LOWERCASE);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(VALID_VIN);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ReturnError_WhenVinIsNullOrEmptyOrWhitespace(string? invalidInput)
    {
        // Act
        var result = Vin.Create(invalidInput);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("VIN Cannot be null or empty");
    }

    [Fact]
    public void Create_Should_ReturnError_WhenVinIsTooShort()
    {
        // Act
        var result = Vin.Create(INVALID_VIN_SHORT);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("exactly 17 characters long");
    }

    [Fact]
    public void Create_Should_ReturnError_WhenVinIsTooLong()
    {
        // Act
        var result = Vin.Create(INVALID_VIN_LONG);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("exactly 17 characters long");
    }

    [Fact]
    public void Create_Should_ReturnError_WhenVinContainsInvalidCharacters()
    {
        // Act
        var result = Vin.Create(INVALID_VIN_CHARS);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("invalid characters");
    }

    [Fact]
    public void ImplicitConversion_ToString_Should_ReturnValue()
    {
        // Arrange
        var vinResult = Vin.Create(VALID_VIN);
        vinResult.HasError().Should().BeFalse();
        var vin = vinResult.Value!;

        // Act
        string vinString = vin;

        // Assert
        vinString.Should().Be(VALID_VIN);
    }

    [Fact]
    public void ToString_Should_ReturnValue()
    {
        // Arrange
        var vinResult = Vin.Create(VALID_VIN);
        vinResult.HasError().Should().BeFalse();
        var vin = vinResult.Value!;

        // Act
        var vinString = vin.ToString();

        // Assert
        vinString.Should().Be(VALID_VIN);
    }

    [Fact]
    public void Equality_Should_BeBasedOnValue()
    {
        // Arrange
        var vin1Result = Vin.Create(VALID_VIN);
        var vin2Result = Vin.Create(VALID_VIN_LOWERCASE);
        var vin3Result = Vin.Create("JMZGG128271672202");

        vin1Result.HasError().Should().BeFalse();
        vin2Result.HasError().Should().BeFalse();
        vin3Result.HasError().Should().BeFalse();

        var vin1 = vin1Result.Value!;
        var vin2 = vin2Result.Value!;
        var vin3 = vin3Result.Value!;

        vin1.Should().Be(vin2);
        vin1.Should().NotBe(vin3);
        (vin1 == vin2).Should().BeTrue();
        (vin1 != vin3).Should().BeTrue();
    }
}

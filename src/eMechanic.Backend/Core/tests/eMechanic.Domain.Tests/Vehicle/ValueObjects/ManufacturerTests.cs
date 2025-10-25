namespace eMechanic.Domain.Tests.Vehicle.ValueObjects;

using Domain.Vehicle.ValueObjects;
using Common.Result;
using FluentAssertions;

public class ManufacturerTests
{
    private const string VALID_MANUFACTURER = "Volkswagen";
    private const string MANUFACTURER_WITH_SPACES = " BMW ";
    private const string EXPECTED_TRIMMED_MANUFACTURER = "BMW";
    private const int MAX_LENGTH = 100;
    private static readonly string LONG_MANUFACTURER = new('a', MAX_LENGTH + 1);

    [Fact]
    public void Create_Should_ReturnSuccess_WhenManufacturerIsValid()
    {
        // Act
        var result = Manufacturer.Create(VALID_MANUFACTURER);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(VALID_MANUFACTURER);
    }

    [Fact]
    public void Create_Should_ReturnSuccessAndTrim_WhenManufacturerHasLeadingTrailingSpaces()
    {
        // Act
        var result = Manufacturer.Create(MANUFACTURER_WITH_SPACES);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(EXPECTED_TRIMMED_MANUFACTURER);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ReturnError_WhenManufacturerIsNullOrEmptyOrWhitespace(string? invalidInput)
    {
        // Act
        var result = Manufacturer.Create(invalidInput);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("Manufacturer name cannot be null or empty");
    }

    [Fact]
    public void Create_Should_ReturnError_WhenManufacturerIsTooLong()
    {
        // Act
        var result = Manufacturer.Create(LONG_MANUFACTURER);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain($"cannot exceed {MAX_LENGTH} characters");
    }

    [Fact]
    public void ImplicitConversion_ToString_Should_ReturnValue()
    {
        // Arrange
        var result = Manufacturer.Create(VALID_MANUFACTURER);
        result.HasError().Should().BeFalse();
        var manufacturer = result.Value!;

        // Act
        string manufacturerString = manufacturer;

        // Assert
        manufacturerString.Should().Be(VALID_MANUFACTURER);
    }

    [Fact]
    public void ToString_Should_ReturnValue()
    {
        // Arrange
        var result = Manufacturer.Create(VALID_MANUFACTURER);
        result.HasError().Should().BeFalse();
        var manufacturer = result.Value!;

        // Act
        var manufacturerString = manufacturer.ToString();

        // Assert
        manufacturerString.Should().Be(VALID_MANUFACTURER);
    }

    [Fact]
    public void Equality_Should_BeBasedOnValue()
    {
        // Arrange
        var m1Result = Manufacturer.Create(VALID_MANUFACTURER);
        var m2Result = Manufacturer.Create(VALID_MANUFACTURER);
        var m3Result = Manufacturer.Create("Audi");

        m1Result.HasError().Should().BeFalse();
        m2Result.HasError().Should().BeFalse();
        m3Result.HasError().Should().BeFalse();

        var m1 = m1Result.Value!;
        var m2 = m2Result.Value!;
        var m3 = m3Result.Value!;

        // Assert
        m1.Should().Be(m2);
        m1.Should().NotBe(m3);
        (m1 == m2).Should().BeTrue();
        (m1 != m3).Should().BeTrue();
    }
}

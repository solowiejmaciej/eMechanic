namespace eMechanic.Domain.Tests.Vehicle.ValueObjects;

using Domain.Vehicle.ValueObjects;
using Common.Result;
using FluentAssertions;

public class ModelTests
{
    private const string VALID_MODEL = "Golf";
    private const string MODEL_WITH_SPACES = " Passat ";
    private const string EXPECTED_TRIMMED_MODEL = "Passat";
    private const int MAX_LENGTH = 100;
    private static readonly string LONG_MODEL = new('b', MAX_LENGTH + 1);

    [Fact]
    public void Create_Should_ReturnSuccess_WhenModelIsValid()
    {
        // Act
        var result = Model.Create(VALID_MODEL);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(VALID_MODEL);
    }

     [Fact]
    public void Create_Should_ReturnSuccessAndTrim_WhenModelHasLeadingTrailingSpaces()
    {
        // Act
        var result = Model.Create(MODEL_WITH_SPACES);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(EXPECTED_TRIMMED_MODEL);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ReturnError_WhenModelIsNullOrEmptyOrWhitespace(string? invalidInput)
    {
        // Act
        var result = Model.Create(invalidInput);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("Model cannot be null or empty");
    }

    [Fact]
    public void Create_Should_ReturnError_WhenModelIsTooLong()
    {
        // Act
        var result = Model.Create(LONG_MODEL);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain($"cannot exceed {MAX_LENGTH} characters");
    }

    [Fact]
    public void ImplicitConversion_ToString_Should_ReturnValue()
    {
        // Arrange
        var result = Model.Create(VALID_MODEL);
        result.HasError().Should().BeFalse();
        var model = result.Value!;

        // Act
        string modelString = model;

        // Assert
        modelString.Should().Be(VALID_MODEL);
    }

    [Fact]
    public void ToString_Should_ReturnValue()
    {
        // Arrange
        var result = Model.Create(VALID_MODEL);
        result.HasError().Should().BeFalse();
        var model = result.Value!;

        // Act
        var modelString = model.ToString();

        // Assert
        modelString.Should().Be(VALID_MODEL);
    }

    [Fact]
    public void Equality_Should_BeBasedOnValue()
    {
        // Arrange
        var m1Result = Model.Create(VALID_MODEL);
        var m2Result = Model.Create(VALID_MODEL);
        var m3Result = Model.Create("Tiguan");

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

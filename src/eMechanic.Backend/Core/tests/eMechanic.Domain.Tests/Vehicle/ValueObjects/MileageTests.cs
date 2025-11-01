namespace eMechanic.Domain.Tests.Vehicle.ValueObjects;


using Domain.Vehicle.ValueObjects;
using Common.Result;
using FluentAssertions;
using eMechanic.Domain.Vehicle.Enums;

public class MileageTests
{
    [Theory]
    [InlineData(100000, EMileageUnit.Kilometers)]
    [InlineData(0, EMileageUnit.Miles)]
    [InlineData(60000, EMileageUnit.Miles)]
    public void Create_Should_ReturnSuccess_WhenMileageIsValid(int validValue, EMileageUnit validUnit)
    {
        // Act
        var result = Mileage.Create(validValue, validUnit);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(validValue);
        result.Value.Unit.Should().Be(validUnit);
    }

    [Fact]
    public void Create_Should_ReturnError_WhenValueIsNull()
    {
        // Act
        var result = Mileage.Create(null, EMileageUnit.Kilometers);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("Mileage cannot be null");
    }

    [Fact]
    public void Create_Should_ReturnError_WhenValueIsNegative()
    {
        // Act
        var result = Mileage.Create(-50, EMileageUnit.Kilometers);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
    }

    [Fact]
    public void Create_Should_ReturnError_WhenUnitIsNone()
    {
        // Act
        var result = Mileage.Create(10000, EMileageUnit.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("Unit can't be None");
    }

    [Fact]
    public void Equality_Should_BeBasedOnValueAndUnit()
    {
        // Arrange
        var m1 = Mileage.Create(100, EMileageUnit.Kilometers).Value;
        var m2 = Mileage.Create(100, EMileageUnit.Kilometers).Value;
        var m3 = Mileage.Create(101, EMileageUnit.Kilometers).Value;
        var m4 = Mileage.Create(100, EMileageUnit.Miles).Value;

        // Assert
        m1.Should().Be(m2);
        m1.Should().NotBe(m3);
        m1.Should().NotBe(m4);
        (m1 == m2).Should().BeTrue();
        (m1 != m3).Should().BeTrue();
    }
}

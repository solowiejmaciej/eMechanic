namespace eMechanic.Domain.Tests.Vehicle.ValueObjects;

using Common.Result;
using Domain.Vehicle.ValueObjects;
using FluentAssertions;

public class EngineCapacityTests
{
    private const decimal VALID_CAPACITY = 2.0m;
    private const decimal ZERO_CAPACITY = 0m;
    private const decimal NEGATIVE_CAPACITY = -1.6m;

    [Fact]
    public void Create_Should_ReturnSuccess_WhenCapacityIsValid()
    {
        // Act
        var result = EngineCapacity.Create(VALID_CAPACITY);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(VALID_CAPACITY);
    }

    [Fact]
    public void Create_Should_ReturnError_WhenCapacityIsZero()
    {
        // Act
        var result = EngineCapacity.Create(ZERO_CAPACITY);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("must be a positive value");
    }

    [Fact]
    public void Create_Should_ReturnError_WhenCapacityIsNegative()
    {
        // Act
        var result = EngineCapacity.Create(NEGATIVE_CAPACITY);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("must be a positive value");
    }

    [Fact]
    public void ImplicitConversion_ToDecimal_Should_ReturnValue()
    {
        // Arrange
        var result = EngineCapacity.Create(VALID_CAPACITY);
        result.HasError().Should().BeFalse();
        var capacity = result.Value!;

        // Act
        decimal capacityDecimal = capacity;

        // Assert
        capacityDecimal.Should().Be(VALID_CAPACITY);
    }

    [Fact]
    public void Equality_Should_BeBasedOnValue()
    {
        // Arrange
        var ec1Result = EngineCapacity.Create(2.0m);
        var ec2Result = EngineCapacity.Create(2.00m);
        var ec3Result = EngineCapacity.Create(1.6m);

        ec1Result.HasError().Should().BeFalse();
        ec2Result.HasError().Should().BeFalse();
        ec3Result.HasError().Should().BeFalse();

        var ec1 = ec1Result.Value!;
        var ec2 = ec2Result.Value!;
        var ec3 = ec3Result.Value!;

        // Assert
        ec1.Should().Be(ec2);
        ec1.Should().NotBe(ec3);
        (ec1 == ec2).Should().BeTrue();
        (ec1 != ec3).Should().BeTrue();
    }
}

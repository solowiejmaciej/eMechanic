namespace eMechanic.Domain.Tests.Shared.ValueObjects;

using Common.Result;
using Domain.Shared.ValueObjects;
using FluentAssertions;
using Xunit;

public class MoneyTests
{
    [Fact]
    public void Create_Should_ReturnSuccess_WhenAmountAndCurrencyAreValid()
    {
        // Act
        var result = Money.Create(100.50m, "USD");

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value!.Amount.Should().Be(100.50m);
        result.Value.Currency.Should().Be("USD");
    }

    [Fact]
    public void Create_Should_ReturnSuccess_WhenCurrencyIsDefault()
    {
        // Act
        var result = Money.Create(50);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value!.Currency.Should().Be("PLN");
    }

    [Fact]
    public void Create_Should_ReturnError_WhenAmountIsNegative()
    {
        // Act
        var result = Money.Create(-10m);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("negative");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_Should_ReturnError_WhenCurrencyIsEmpty(string? invalidCurrency)
    {
        // Act
        var result = Money.Create(100, invalidCurrency!);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Message.Should().Contain("Currency code cannot be empty");
    }

    [Theory]
    [InlineData("PL")]
    [InlineData("PLNX")]
    public void Create_Should_ReturnError_WhenCurrencyLengthIsInvalid(string invalidCurrency)
    {
        // Act
        var result = Money.Create(100, invalidCurrency);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Message.Should().Contain("must be 3 characters long");
    }

    [Fact]
    public void Zero_Should_ReturnMoneyWithZeroAmount()
    {
        // Act
        var money = Money.Zero("EUR");

        // Assert
        money.Amount.Should().Be(0m);
        money.Currency.Should().Be("EUR");
    }
}

namespace eMechanic.Application.Tests.Payments.Features.Initialize;

using Application.Payments.Abstractions;
using Application.Payments.Common;
using Application.Payments.Features.Initialize;
using Application.Payments.Strategies;
using Common.Result;
using Domain.Payment.Enums;
using Domain.Shared.ValueObjects;
using FluentAssertions;
using NSubstitute;

public class InitializePaymentCommandHandlerTests
{
    private readonly IPaymentInitializationStrategy _repairStrategy =
        Substitute.For<IPaymentInitializationStrategy>();
    private readonly IPaymentOrderProcessor _paymentOrderProcessor = Substitute.For<IPaymentOrderProcessor>();
    private readonly InitializePaymentCommandHandler _handler;

    public InitializePaymentCommandHandlerTests()
    {
        _repairStrategy.SupportedType.Returns(EPayableType.Repair);

        _handler = new InitializePaymentCommandHandler(
            [_repairStrategy],
            _paymentOrderProcessor);
    }

    [Fact]
    public async Task Handle_Should_ReturnSession_WhenStrategyAndProcessorSucceed()
    {
        // Arrange
        var referenceId = Guid.NewGuid();
        var payableItem = new PayableItem(
            referenceId,
            EPayableType.Repair,
            Money.Create(2000m, "PLN").Value!,
            Guid.NewGuid());

        var expectedSession = new PaymentSessionDto("sess_123", "https://checkout.stripe.com/pay/sess_123");

        _repairStrategy
            .BuildPayableItemAsync(referenceId, Arg.Any<CancellationToken>())
            .Returns(payableItem);

        _paymentOrderProcessor
            .CreateOrGetPendingAsync(payableItem, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(expectedSession);

        var command = new InitializePaymentCommand(
            referenceId,
            EPayableType.Repair,
            "https://success.example.com",
            "https://cancel.example.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.SessionId.Should().Be("sess_123");
    }

    [Fact]
    public async Task Handle_Should_ReturnValidationError_WhenNoStrategyMatchesType()
    {
        // Arrange
        var handlerWithNoStrategies = new InitializePaymentCommandHandler([], _paymentOrderProcessor);

        var command = new InitializePaymentCommand(
            Guid.NewGuid(),
            EPayableType.Repair,
            "https://success.example.com",
            "https://cancel.example.com");

        // Act
        var result = await handlerWithNoStrategies.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        await _paymentOrderProcessor.DidNotReceive()
            .CreateOrGetPendingAsync(Arg.Any<PayableItem>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_PropagateError_WhenStrategyFails()
    {
        // Arrange
        _repairStrategy
            .BuildPayableItemAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new Error(EErrorCode.NotFoundError, "Repair not found."));

        var command = new InitializePaymentCommand(
            Guid.NewGuid(),
            EPayableType.Repair,
            "https://success.example.com",
            "https://cancel.example.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(EErrorCode.NotFoundError);
        await _paymentOrderProcessor.DidNotReceive()
            .CreateOrGetPendingAsync(Arg.Any<PayableItem>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_PropagateError_WhenProcessorFails()
    {
        // Arrange
        var referenceId = Guid.NewGuid();
        var payableItem = new PayableItem(
            referenceId,
            EPayableType.Repair,
            Money.Create(2000m, "PLN").Value!,
            Guid.NewGuid());

        _repairStrategy
            .BuildPayableItemAsync(referenceId, Arg.Any<CancellationToken>())
            .Returns(payableItem);

        _paymentOrderProcessor
            .CreateOrGetPendingAsync(payableItem, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new Error(EErrorCode.ValidationError, "Provider unavailable."));

        var command = new InitializePaymentCommand(
            referenceId,
            EPayableType.Repair,
            "https://success.example.com",
            "https://cancel.example.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
    }
}

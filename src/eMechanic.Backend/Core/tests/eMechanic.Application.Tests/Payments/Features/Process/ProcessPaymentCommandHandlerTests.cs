namespace eMechanic.Application.Tests.Payments.Features.Process;

using Application.Payments.Abstractions;
using Application.Payments.Common;
using Application.Payments.Features.Process;
using Application.Payments.Notifications;
using Common.Result;
using Domain.Payment.Enums;
using FluentAssertions;
using MediatR;
using NSubstitute;

public class ProcessPaymentCommandHandlerTests
{
    private readonly IPaymentProcessor _paymentProcessor = Substitute.For<IPaymentProcessor>();
    private readonly IPaymentOrderProcessor _paymentOrderProcessor = Substitute.For<IPaymentOrderProcessor>();
    private readonly IPublisher _publisher = Substitute.For<IPublisher>();
    private readonly ProcessPaymentCommandHandler _handler;

    public ProcessPaymentCommandHandlerTests()
    {
        _handler = new ProcessPaymentCommandHandler(
            _paymentProcessor,
            _paymentOrderProcessor,
            _publisher);
    }

    [Fact]
    public async Task Handle_Should_PublishNotification_WhenOrderNewlyCompleted()
    {
        var referenceId = Guid.NewGuid();
        var payload = new PaymentProcessorPayload(referenceId, EPayableType.Repair, "cs_test_123");
        var completion = new PaymentOrderCompletionResult(referenceId, EPayableType.Repair, true);

        _paymentProcessor
            .ProcessAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(payload);

        _paymentOrderProcessor
            .CompleteAsync(payload, Arg.Any<CancellationToken>())
            .Returns(completion);

        var result = await _handler.Handle(
            new ProcessPaymentCommand("{}", "t=1,v1=sig"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _publisher.Received(1).Publish(
            Arg.Is<PaymentConfirmedNotification>(n => n.ReferenceId == completion.ReferenceId && n.Type == completion.Type),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessWithoutPublishing_WhenOrderAlreadyCompleted()
    {
        var referenceId = Guid.NewGuid();
        var payload = new PaymentProcessorPayload(referenceId, EPayableType.Repair, "cs_test_retry");
        var completion = new PaymentOrderCompletionResult(referenceId, EPayableType.Repair, false);

        _paymentProcessor
            .ProcessAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(payload);

        _paymentOrderProcessor
            .CompleteAsync(payload, Arg.Any<CancellationToken>())
            .Returns(completion);

        var result = await _handler.Handle(
            new ProcessPaymentCommand("{}", "sig"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _publisher.DidNotReceive().Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenProcessorFails()
    {
        _paymentProcessor
            .ProcessAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new Error(EErrorCode.ValidationError, "Invalid Stripe signature."));

        var result = await _handler.Handle(
            new ProcessPaymentCommand("{}", "invalid"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        await _publisher.DidNotReceive().Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenOrderCompletionFails()
    {
        var payload = new PaymentProcessorPayload(Guid.NewGuid(), EPayableType.Repair, "cs_not_found");

        _paymentProcessor
            .ProcessAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(payload);

        _paymentOrderProcessor
            .CompleteAsync(payload, Arg.Any<CancellationToken>())
            .Returns(new Error(EErrorCode.NotFoundError, "PaymentOrder not found."));

        var result = await _handler.Handle(
            new ProcessPaymentCommand("{}", "sig"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(EErrorCode.NotFoundError);
    }
}

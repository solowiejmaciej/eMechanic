namespace eMechanic.Application.Tests.Payments.Features.ProcessWebhook;

using Application.Payments.Abstractions;
using Application.Payments.Common;
using Application.Payments.Features.ProcessWebhook;
using Application.Payments.Notifications;
using Common.Result;
using Domain.Shared.ValueObjects;
using FluentAssertions;
using MediatR;
using NSubstitute;

public class ProcessPaymentWebhookCommandHandlerTests
{
    private readonly IPaymentWebhookProcessor _webhookProcessor = Substitute.For<IPaymentWebhookProcessor>();
    private readonly IPublisher _publisher = Substitute.For<IPublisher>();
    private readonly ProcessPaymentWebhookCommandHandler _handler;

    public ProcessPaymentWebhookCommandHandlerTests()
    {
        _handler = new ProcessPaymentWebhookCommandHandler(_webhookProcessor, _publisher);
    }

    [Fact]
    public async Task Handle_Should_PublishNotification_WhenProcessorSucceeds()
    {
        // Arrange
        var repairId = Guid.NewGuid();
        var payerId = Guid.NewGuid();
        var money = Money.Create(2000m, "PLN").Value!;
        var payableItem = new PayableItem(repairId, EPayableType.Repair, money, payerId);

        _webhookProcessor
            .ProcessAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(payableItem);

        var command = new ProcessPaymentWebhookCommand("{}", "t=1234,v1=sig");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await _publisher.Received(1).Publish(
            Arg.Is<PaymentConfirmedNotification>(n =>
                n.ReferenceId == repairId && n.Type == EPayableType.Repair),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenProcessorFails()
    {
        // Arrange
        var processorError = new Error(EErrorCode.ValidationError, "Invalid Stripe signature.");

        _webhookProcessor
            .ProcessAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(processorError);

        var command = new ProcessPaymentWebhookCommand("{}", "invalid-sig");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);

        await _publisher.DidNotReceive().Publish(
            Arg.Any<INotification>(),
            Arg.Any<CancellationToken>());
    }
}


namespace eMechanic.Application.Tests.Payments.Services;

using Application.Payments.Abstractions;
using Application.Payments.Common;
using Application.Payments.Repositories;
using Application.Payments.Services;
using Common.Result;
using Domain.Payment;
using Domain.Payment.Enums;
using Domain.Shared.ValueObjects;
using FluentAssertions;
using NSubstitute;

public class PaymentOrderProcessorTests
{
    private readonly IPaymentOrderRepository _paymentOrderRepository = Substitute.For<IPaymentOrderRepository>();
    private readonly IPaymentProcessor _paymentProcessor = Substitute.For<IPaymentProcessor>();
    private readonly PaymentOrderProcessor _processor;

    public PaymentOrderProcessorTests()
    {
        _processor = new PaymentOrderProcessor(_paymentOrderRepository, _paymentProcessor);
    }

    [Fact]
    public async Task CreateOrGetPendingAsync_Should_ReturnExistingPendingOrder_WhenExists()
    {
        var payableItem = new PayableItem(Guid.NewGuid(), EPayableType.Repair, Money.Create(100m, "PLN").Value!, Guid.NewGuid());
        var existing = PaymentOrder.Create(
            payableItem.ReferenceId,
            EPayableType.Repair,
            payableItem.Amount,
            payableItem.PayerId);
        existing.StartCheckout("sess_existing", "https://checkout.stripe.com/pay/existing");

        _paymentOrderRepository
            .GetActiveByReferenceAsync(payableItem.ReferenceId, EPayableType.Repair, Arg.Any<CancellationToken>())
            .Returns(existing);

        var result = await _processor.CreateOrGetPendingAsync(
            payableItem,
            "https://ok",
            "https://cancel",
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.SessionId.Should().Be("sess_existing");
        await _paymentProcessor.DidNotReceive()
            .CreateCheckoutSessionAsync(Arg.Any<PaymentOrder>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateOrGetPendingAsync_Should_CreateOrder_WhenNoPendingOrderExists()
    {
        var payableItem = new PayableItem(Guid.NewGuid(), EPayableType.Repair, Money.Create(100m, "PLN").Value!, Guid.NewGuid());
        var session = new PaymentSessionDto("sess_new", "https://checkout.stripe.com/pay/new");

        _paymentOrderRepository
            .GetActiveByReferenceAsync(payableItem.ReferenceId, EPayableType.Repair, Arg.Any<CancellationToken>())
            .Returns((PaymentOrder?)null);

        _paymentProcessor
            .CreateCheckoutSessionAsync(Arg.Any<PaymentOrder>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(session);

        var result = await _processor.CreateOrGetPendingAsync(
            payableItem,
            "https://ok",
            "https://cancel",
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _paymentOrderRepository.Received(1).AddAsync(Arg.Any<PaymentOrder>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateOrGetPendingAsync_Should_PersistAndMarkFailed_WhenProviderReturnsErrorForNewOrder()
    {
        var payableItem = new PayableItem(Guid.NewGuid(), EPayableType.Repair, Money.Create(100m, "PLN").Value!, Guid.NewGuid());
        var providerError = new Error(EErrorCode.InternalServerError, "stripe unavailable");

        _paymentOrderRepository
            .GetActiveByReferenceAsync(payableItem.ReferenceId, EPayableType.Repair, Arg.Any<CancellationToken>())
            .Returns((PaymentOrder?)null);

        PaymentOrder? addedOrder = null;
        _paymentOrderRepository
            .AddAsync(Arg.Do<PaymentOrder>(order => addedOrder = order), Arg.Any<CancellationToken>())
            .Returns(Guid.NewGuid());

        _paymentProcessor
            .CreateCheckoutSessionAsync(Arg.Any<PaymentOrder>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(providerError);

        var result = await _processor.CreateOrGetPendingAsync(
            payableItem,
            "https://ok",
            "https://cancel",
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(providerError);
        addedOrder.Should().NotBeNull();
        addedOrder!.Status.Should().Be(EPaymentOrderStatus.Failed);

        await _paymentOrderRepository.Received(1).AddAsync(Arg.Any<PaymentOrder>(), Arg.Any<CancellationToken>());
        await _paymentOrderRepository.Received(2).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateOrGetPendingAsync_Should_MarkExistingOrderAsFailed_WhenProviderReturnsError()
    {
        var payableItem = new PayableItem(Guid.NewGuid(), EPayableType.Repair, Money.Create(100m, "PLN").Value!, Guid.NewGuid());
        var existingOrder = PaymentOrder.Create(
            payableItem.ReferenceId,
            EPayableType.Repair,
            payableItem.Amount,
            payableItem.PayerId);
        var providerError = new Error(EErrorCode.InternalServerError, "stripe unavailable");

        _paymentOrderRepository
            .GetActiveByReferenceAsync(payableItem.ReferenceId, EPayableType.Repair, Arg.Any<CancellationToken>())
            .Returns(existingOrder);

        _paymentProcessor
            .CreateCheckoutSessionAsync(existingOrder, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(providerError);

        var result = await _processor.CreateOrGetPendingAsync(
            payableItem,
            "https://ok",
            "https://cancel",
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(providerError);
        existingOrder.Status.Should().Be(EPaymentOrderStatus.Failed);

        await _paymentOrderRepository.DidNotReceive().AddAsync(Arg.Any<PaymentOrder>(), Arg.Any<CancellationToken>());
        await _paymentOrderRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CompleteAsync_Should_BeIdempotent_WhenAlreadyPaid()
    {
        var referenceId = Guid.NewGuid();
        var order = PaymentOrder.Create(
            referenceId,
            EPayableType.Repair,
            Money.Create(100m, "PLN").Value!,
            Guid.NewGuid());
        order.StartCheckout("sess_paid", "https://checkout.stripe.com/pay/paid");
        order.Complete();

        _paymentOrderRepository
            .GetByProviderSessionIdAsync("sess_paid", Arg.Any<CancellationToken>())
            .Returns(order);

        var result = await _processor.CompleteAsync(
            new PaymentProcessorPayload(referenceId, EPayableType.Repair, "sess_paid"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.IsNewlyCompleted.Should().BeFalse();
        await _paymentOrderRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

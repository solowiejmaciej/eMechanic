namespace eMechanic.Application.Payments.Services;

using Abstractions;
using Common;
using eMechanic.Application.Payments.Repositories;
using eMechanic.Common.Result;
using eMechanic.Domain.Payment;
using eMechanic.Domain.Payment.Enums;

public sealed class PaymentOrderProcessor : IPaymentOrderProcessor
{
    private readonly IPaymentOrderRepository _paymentOrderRepository;
    private readonly IPaymentProcessor _paymentProcessor;

    public PaymentOrderProcessor(
        IPaymentOrderRepository paymentOrderRepository,
        IPaymentProcessor paymentProcessor)
    {
        _paymentOrderRepository = paymentOrderRepository;
        _paymentProcessor = paymentProcessor;
    }

    public async Task<Result<PaymentSessionDto, Error>> CreateOrGetPendingAsync(
        PayableItem payableItem,
        string successUrl,
        string cancelUrl,
        CancellationToken cancellationToken)
    {
        var existingOrder = await _paymentOrderRepository.GetActiveByReferenceAsync(
            payableItem.ReferenceId,
            payableItem.Type,
            cancellationToken);

        if (existingOrder is not null)
        {
            if (!string.IsNullOrWhiteSpace(existingOrder.ProviderSessionId) &&
                !string.IsNullOrWhiteSpace(existingOrder.CheckoutUrl))
            {
                return new PaymentSessionDto(existingOrder.ProviderSessionId, existingOrder.CheckoutUrl);
            }

            var existingSessionResult = await _paymentProcessor.CreateCheckoutSessionAsync(
                existingOrder,
                successUrl,
                cancelUrl,
                cancellationToken);

            if (existingSessionResult.HasError())
            {
                var markFailedResult = existingOrder.MarkFailed();
                if (!markFailedResult.HasError())
                {
                    await _paymentOrderRepository.SaveChangesAsync(cancellationToken);
                }
                return existingSessionResult.Error!;
            }

            var existingSession = existingSessionResult.Value!;
            var startExistingResult = existingOrder.StartCheckout(existingSession.SessionId, existingSession.CheckoutUrl);

            if (startExistingResult.HasError())
            {
                return startExistingResult.Error!;
            }

            await _paymentOrderRepository.SaveChangesAsync(cancellationToken);
            return existingSession;
        }

        var paymentOrder = PaymentOrder.Create(
            payableItem.ReferenceId,
            payableItem.Type,
            payableItem.Amount,
            payableItem.PayerId);

        await _paymentOrderRepository.AddAsync(paymentOrder, cancellationToken);
        await _paymentOrderRepository.SaveChangesAsync(cancellationToken);

        var sessionResult = await _paymentProcessor.CreateCheckoutSessionAsync(
            paymentOrder,
            successUrl,
            cancelUrl,
            cancellationToken);

        if (sessionResult.HasError())
        {
            var markFailedResult = paymentOrder.MarkFailed();
            if (!markFailedResult.HasError())
            {
                await _paymentOrderRepository.SaveChangesAsync(cancellationToken);
            }
            return sessionResult.Error!;
        }

        var session = sessionResult.Value!;
        var startCheckoutResult = paymentOrder.StartCheckout(session.SessionId, session.CheckoutUrl);

        if (startCheckoutResult.HasError())
        {
            var markFailedResult = paymentOrder.MarkFailed();
            if (!markFailedResult.HasError())
            {
                await _paymentOrderRepository.SaveChangesAsync(cancellationToken);
            }
            return startCheckoutResult.Error!;
        }

        await _paymentOrderRepository.SaveChangesAsync(cancellationToken);

        return session;
    }

    public async Task<Result<PaymentOrderCompletionResult, Error>> CompleteAsync(
        PaymentProcessorPayload payload,
        CancellationToken cancellationToken)
    {
        var order = await _paymentOrderRepository.GetByProviderSessionIdAsync(payload.ProviderSessionId, cancellationToken)
                    ?? await _paymentOrderRepository.GetByReferenceAndTypeAsync(payload.ReferenceId, payload.Type,
                        cancellationToken);

        if (order is null)
        {
            return new Error(EErrorCode.NotFoundError,
                $"PaymentOrder not found for providerSessionId '{payload.ProviderSessionId}' or ({payload.ReferenceId}, {payload.Type}).");
        }

        if (order.Status == EPaymentOrderStatus.Paid)
        {
            return new PaymentOrderCompletionResult(order.ReferenceId, order.PayableType, false);
        }

        var completeResult = order.Complete();

        if (completeResult.HasError())
        {
            return completeResult.Error!;
        }

        await _paymentOrderRepository.SaveChangesAsync(cancellationToken);

        return new PaymentOrderCompletionResult(order.ReferenceId, order.PayableType, true);
    }
}

namespace eMechanic.Domain.Payment;

using Common.DDD;
using Common.Result;
using eMechanic.Domain.Payment.Enums;
using Shared.ValueObjects;

public sealed class PaymentOrder : AggregateRoot
{
    public string ProviderSessionId { get; private set; } = string.Empty;
    public string CheckoutUrl { get; private set; } = string.Empty;
    public Guid ReferenceId { get; private set; }
    public EPayableType PayableType { get; private set; }
    public Money Amount { get; private set; } = null!;
    public Guid PayerId { get; private set; }
    public EPaymentOrderStatus Status { get; private set; }

    private PaymentOrder() { }

    private PaymentOrder(
        Guid referenceId,
        EPayableType payableType,
        Money amount,
        Guid payerId)
    {
        ReferenceId = referenceId;
        PayableType = payableType;
        Amount = amount;
        PayerId = payerId;
        Status = EPaymentOrderStatus.Created;
    }

    public static PaymentOrder Create(
        Guid referenceId,
        EPayableType payableType,
        Money amount,
        Guid payerId)
    {
        return new PaymentOrder(referenceId, payableType, amount, payerId);
    }

    public Result<Success, Error> StartCheckout(string providerSessionId, string checkoutUrl)
    {
        if (Status != EPaymentOrderStatus.Created)
        {
            return new Error(
                EErrorCode.ValidationError,
                $"PaymentOrder can only start checkout from '{EPaymentOrderStatus.Created}' status. Current: '{Status}'.");
        }

        if (string.IsNullOrWhiteSpace(providerSessionId) || string.IsNullOrWhiteSpace(checkoutUrl))
        {
            return new Error(EErrorCode.ValidationError, "Provider session id and checkout url are required.");
        }

        ProviderSessionId = providerSessionId;
        CheckoutUrl = checkoutUrl;
        Status = EPaymentOrderStatus.CheckoutStarted;
        return Result.Success;
    }

    public Result<Success, Error> Complete()
    {
        if (Status != EPaymentOrderStatus.CheckoutStarted)
        {
            return new Error(
                EErrorCode.ValidationError,
                $"PaymentOrder can only be completed from '{EPaymentOrderStatus.CheckoutStarted}' status. Current: '{Status}'.");
        }

        Status = EPaymentOrderStatus.Paid;
        return Result.Success;
    }

    public Result<Success, Error> Cancel()
    {
        if (Status == EPaymentOrderStatus.Paid)
        {
            return new Error(
                EErrorCode.ValidationError,
                $"Paid PaymentOrder cannot be cancelled. Current: '{Status}'.");
        }

        Status = EPaymentOrderStatus.Cancelled;
        return Result.Success;
    }

    public Result<Success, Error> MarkFailed()
    {
        if (Status == EPaymentOrderStatus.Paid)
        {
            return new Error(EErrorCode.ValidationError, "Paid PaymentOrder cannot be marked as failed.");
        }

        Status = EPaymentOrderStatus.Failed;
        return Result.Success;
    }
}

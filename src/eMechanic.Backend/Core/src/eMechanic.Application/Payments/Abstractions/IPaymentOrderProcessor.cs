namespace eMechanic.Application.Payments.Abstractions;

using Common;
using eMechanic.Common.Result;

public interface IPaymentOrderProcessor
{
    Task<Result<PaymentSessionDto, Error>> CreateOrGetPendingAsync(
        PayableItem payableItem,
        string successUrl,
        string cancelUrl,
        CancellationToken cancellationToken);

    Task<Result<PaymentOrderCompletionResult, Error>> CompleteAsync(
        PaymentProcessorPayload payload,
        CancellationToken cancellationToken);
}

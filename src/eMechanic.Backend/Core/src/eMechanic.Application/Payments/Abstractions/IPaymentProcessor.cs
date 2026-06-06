namespace eMechanic.Application.Payments.Abstractions;

using Common;
using eMechanic.Common.Result;

public interface IPaymentProcessor
{
    Task<Result<PaymentSessionDto, Error>> CreateCheckoutSessionAsync(
        eMechanic.Domain.Payment.PaymentOrder paymentOrder,
        string successUrl,
        string cancelUrl,
        CancellationToken cancellationToken);

    Task<Result<PaymentProcessorPayload, Error>> ProcessAsync(
        string jsonPayload,
        string signatureHeader,
        CancellationToken cancellationToken);
}

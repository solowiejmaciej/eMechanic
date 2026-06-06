namespace eMechanic.Infrastructure.Payments;

using eMechanic.Application.Payments.Common;
using eMechanic.Common.Result;
using eMechanic.Domain.Payment;

internal interface IStripePaymentService
{
    Task<Result<PaymentSessionDto, Error>> CreateCheckoutSessionAsync(
        PaymentOrder paymentOrder,
        string successUrl,
        string cancelUrl,
        CancellationToken cancellationToken);
}


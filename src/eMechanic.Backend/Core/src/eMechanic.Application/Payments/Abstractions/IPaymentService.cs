namespace eMechanic.Application.Payments.Abstractions;

using Common;
using eMechanic.Common.Result;

public interface IPaymentService
{
    Task<Result<PaymentSessionDto, Error>> CreateCheckoutSessionAsync(
        PayableItem item,
        string successUrl,
        string cancelUrl,
        CancellationToken cancellationToken);
}


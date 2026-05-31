namespace eMechanic.Application.Payments.Abstractions;

using Common;
using eMechanic.Common.Result;

public interface IPaymentWebhookProcessor
{
    Task<Result<PayableItem, Error>> ProcessAsync(
        string jsonPayload,
        string signatureHeader,
        CancellationToken cancellationToken);
}


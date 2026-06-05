namespace eMechanic.Application.Payments.Strategies;

using Common;
using Domain.Payment.Enums;
using eMechanic.Common.Result;

public interface IPaymentInitializationStrategy
{
    EPayableType SupportedType { get; }

    Task<Result<PayableItem, Error>> BuildPayableItemAsync(
        Guid referenceId,
        CancellationToken cancellationToken);
}

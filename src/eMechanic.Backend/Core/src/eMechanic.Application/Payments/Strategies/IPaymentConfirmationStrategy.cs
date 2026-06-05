namespace eMechanic.Application.Payments.Strategies;

using Domain.Payment.Enums;

public interface IPaymentConfirmationStrategy
{
    EPayableType SupportedType { get; }

    Task HandleAsync(Guid referenceId, CancellationToken cancellationToken);
}

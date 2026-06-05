namespace eMechanic.Application.Payments.Repositories;

using Domain.Payment.Enums;
using eMechanic.Application.Abstractions.Repositories;
using eMechanic.Domain.Payment;

public interface IPaymentOrderRepository : IRepository<PaymentOrder>
{
    Task<PaymentOrder?> GetByProviderSessionIdAsync(string providerSessionId, CancellationToken cancellationToken);

    Task<PaymentOrder?> GetByReferenceAndTypeAsync(Guid referenceId, EPayableType payableType, CancellationToken cancellationToken);

    Task<PaymentOrder?> GetActiveByReferenceAsync(
        Guid referenceId,
        EPayableType payableType,
        CancellationToken cancellationToken);
}

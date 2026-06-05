namespace eMechanic.Infrastructure.Repositories;

using Application.Payments.Repositories;
using Base;
using DAL;
using Domain.Payment;
using Domain.Payment.Enums;
using Microsoft.EntityFrameworkCore;
using Services;

internal sealed class PaymentOrderRepository : Repository<PaymentOrder>, IPaymentOrderRepository
{
    public PaymentOrderRepository(AppDbContext context, IPaginationService paginationService)
        : base(context, paginationService)
    {
    }

    public Task<PaymentOrder?> GetByProviderSessionIdAsync(string providerSessionId, CancellationToken cancellationToken)
    {
        return GetQuery()
            .SingleOrDefaultAsync(p => p.ProviderSessionId == providerSessionId, cancellationToken);
    }

    public Task<PaymentOrder?> GetByReferenceAndTypeAsync(Guid referenceId, EPayableType payableType, CancellationToken cancellationToken)
    {
        return GetQuery()
            .Where(p => p.ReferenceId == referenceId && p.PayableType == payableType)
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<PaymentOrder?> GetActiveByReferenceAsync(
        Guid referenceId,
        EPayableType payableType,
        CancellationToken cancellationToken)
    {
        return GetQuery()
            .SingleOrDefaultAsync(
                p => p.ReferenceId == referenceId
                     && p.PayableType == payableType
                     && (p.Status == EPaymentOrderStatus.Created || p.Status == EPaymentOrderStatus.CheckoutStarted),
                cancellationToken);
    }
}

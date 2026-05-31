namespace eMechanic.Application.Repair.PaymentHandlers;

using eMechanic.Application.Payments.Common;
using eMechanic.Application.Payments.Notifications;
using eMechanic.Application.Repair.Repositories;
using MediatR;

public sealed class RepairPaymentConfirmedHandler : INotificationHandler<PaymentConfirmedNotification>
{
    private readonly IRepairRepository _repairRepository;

    public RepairPaymentConfirmedHandler(IRepairRepository repairRepository)
    {
        _repairRepository = repairRepository;
    }

    public async Task Handle(PaymentConfirmedNotification notification, CancellationToken cancellationToken)
    {
        if (notification.Type != EPayableType.Repair)
        {
            return;
        }

        var repair = await _repairRepository.GetByIdAsync(notification.ReferenceId, cancellationToken);

        if (repair is null)
        {
            return;
        }

        var payResult = repair.Pay();

        if (payResult.HasError())
        {
            return;
        }

        await _repairRepository.SaveChangesAsync(cancellationToken);
    }
}


namespace eMechanic.Application.Repair.PaymentStrategies;

using Domain.Payment.Enums;
using eMechanic.Application.Payments.Strategies;
using eMechanic.Application.Repair.Repositories;

public sealed class RepairPaymentConfirmationStrategy : IPaymentConfirmationStrategy
{
    private readonly IRepairRepository _repairRepository;

    public EPayableType SupportedType => EPayableType.Repair;

    public RepairPaymentConfirmationStrategy(IRepairRepository repairRepository)
    {
        _repairRepository = repairRepository;
    }

    public async Task HandleAsync(Guid referenceId, CancellationToken cancellationToken)
    {
        var repair = await _repairRepository.GetByIdAsync(referenceId, cancellationToken);

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

namespace eMechanic.Application.Repair.Features.Pay;

using Common.CQRS;
using Common.Result;
using Repositories;

public sealed class PayForRepairCommandHandler : IResultCommandHandler<PayForRepairCommand, Success>
{
    private readonly IRepairRepository _repairRepository;

    public PayForRepairCommandHandler(IRepairRepository repairRepository)
    {
        _repairRepository = repairRepository;
    }

    public async Task<Result<Success, Error>> Handle(PayForRepairCommand request, CancellationToken cancellationToken)
    {
        var repair = await _repairRepository.GetByIdAsync(request.RepairId, cancellationToken);

        if (repair is null)
        {
            return new Error(EErrorCode.NotFoundError, $"Repair with ID {request.RepairId} not found.");
        }

        var payResult = repair.Pay();

        if (payResult.HasError())
        {
            return payResult.Error!;
        }

        await _repairRepository.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}


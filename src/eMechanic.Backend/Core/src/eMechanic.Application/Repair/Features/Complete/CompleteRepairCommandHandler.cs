namespace eMechanic.Application.Repair.Features.Complete;

using Abstractions.Identity.Contexts;
using Common.CQRS;
using Common.Result;
using Domain.Shared.ValueObjects;
using Repositories;

public sealed class CompleteRepairCommandHandler : IResultCommandHandler<CompleteRepairCommand, Success>
{
    private readonly IRepairRepository _repairRepository;
    private readonly IWorkshopContext _workshopContext;

    public CompleteRepairCommandHandler(IRepairRepository repairRepository, IWorkshopContext workshopContext)
    {
        _repairRepository = repairRepository;
        _workshopContext = workshopContext;
    }

    public async Task<Result<Success, Error>> Handle(CompleteRepairCommand request, CancellationToken cancellationToken)
    {
        var workshopId = _workshopContext.GetWorkshopId();
        var repair = await _repairRepository.GetByIdAsync(request.RepairId, cancellationToken);

        if (repair is null)
        {
            return new Error(EErrorCode.NotFoundError, $"Repair with ID {request.RepairId} not found.");
        }

        if (repair.WorkshopId != workshopId)
        {
            return new Error(EErrorCode.UnauthorizedError, "Workshop is not assigned to this repair.");
        }

        var moneyResult = Money.Create(request.Amount, request.Currency);

        if (moneyResult.HasError())
        {
            return moneyResult.Error!;
        }

        var completeResult = repair.CompleteRepair(moneyResult.Value!);

        if (completeResult.HasError())
        {
            return completeResult.Error!;
        }

        await _repairRepository.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}


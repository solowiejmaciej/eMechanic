namespace eMechanic.Application.Repair.Features.Start;

using Abstractions.Identity.Contexts;
using Common.CQRS;
using Common.Result;
using Repositories;

public sealed class StartRepairCommandHandler : IResultCommandHandler<StartRepairCommand, Success>
{
    private readonly IRepairRepository _repairRepository;
    private readonly IWorkshopContext _workshopContext;

    public StartRepairCommandHandler(IRepairRepository repairRepository, IWorkshopContext workshopContext)
    {
        _repairRepository = repairRepository;
        _workshopContext = workshopContext;
    }

    public async Task<Result<Success, Error>> Handle(StartRepairCommand request, CancellationToken cancellationToken)
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

        var startResult = repair.StartRepair();

        if (startResult.HasError())
        {
            return startResult.Error!;
        }

        await _repairRepository.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}


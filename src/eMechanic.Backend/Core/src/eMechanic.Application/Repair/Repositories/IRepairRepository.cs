namespace eMechanic.Application.Repair.Repositories;

using Abstractions.Repositories;
using RepairAggregate = eMechanic.Domain.Repair.Repair;

public interface IRepairRepository : IRepository<RepairAggregate>
{
	Task<RepairAggregate?> GetForWorkshopByIdAsNoTrackingAsync(Guid workshopId, Guid repairId, CancellationToken cancellationToken);
	Task<RepairAggregate?> GetForUserByIdAsNoTrackingAsync(Guid userId, Guid repairId, CancellationToken cancellationToken);
}


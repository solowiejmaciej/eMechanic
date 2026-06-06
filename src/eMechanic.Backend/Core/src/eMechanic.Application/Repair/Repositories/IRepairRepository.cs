namespace eMechanic.Application.Repair.Repositories;

using Abstractions.Repositories;
using Common.Result;
using RepairAggregate = eMechanic.Domain.Repair.Repair;

public interface IRepairRepository : IRepository<RepairAggregate>
{
	Task<RepairAggregate?> GetForWorkshopByIdAsNoTrackingAsync(Guid workshopId, Guid repairId, CancellationToken cancellationToken);
    Task<RepairAggregate?> GetForUserByIdAsNoTrackingAsync(Guid userId, Guid requestRepairId, CancellationToken cancellationToken);
	Task<PaginationResult<RepairAggregate>> GetForWorkshopPaginatedAsync(Guid workshopId, PaginationParameters paginationParameters, CancellationToken cancellationToken);
    Task<PaginationResult<RepairAggregate>> GetForUserPaginatedAsync(Guid userId, PaginationParameters paginationParameters, CancellationToken cancellationToken);
}


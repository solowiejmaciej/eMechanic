
namespace eMechanic.Application.RepairRequest.Repositories;

using eMechanic.Application.Abstractions.Repositories;
using eMechanic.Common.Result;
using eMechanic.Domain.RepairRequest;

public interface IRepairRequestRepository : IRepository<RepairRequest>
{
    Task<PaginationResult<RepairRequest>> GetForUserVehicleAsync(Guid vehicleId,
        PaginationParameters paginationParameters, CancellationToken cancellationToken);

    Task<PaginationResult<RepairRequest>> GetForWorkshopAsync(Guid workshopId,
        PaginationParameters paginationParameters, CancellationToken cancellationToken);
}

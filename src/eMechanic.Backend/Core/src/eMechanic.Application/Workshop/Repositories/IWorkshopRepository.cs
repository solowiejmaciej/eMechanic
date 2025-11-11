namespace eMechanic.Application.Workshop.Repositories;

using eMechanic.Application.Abstractions.Repositories;
using eMechanic.Common.Result;
using eMechanic.Domain.Workshop;

public interface IWorkshopRepository : IRepository<Workshop>
{
    Task<Workshop?> GetByIdentityIdAsync(Guid identityId);
    Task<PaginationResult<Workshop>> GetPaginatedAsync(PaginationParameters paginationParameters, CancellationToken cancellationToken);
}

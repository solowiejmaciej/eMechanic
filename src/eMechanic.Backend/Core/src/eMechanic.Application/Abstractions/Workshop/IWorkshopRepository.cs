namespace eMechanic.Application.Abstractions.Workshop;

using Domain.Workshop;
using Repositories;

public interface IWorkshopRepository : IRepository<Workshop>
{
    Task<Workshop?> GetByIdentityIdAsync(Guid identityId);
}

namespace eMechanic.Application.UserRepairPreferences.Repositories;

using eMechanic.Application.Abstractions.Repositories;
using eMechanic.Domain.UserRepairPreferences;

public interface IUserRepairPreferencesRepository : IRepository<UserRepairPreferences>
{
    Task<UserRepairPreferences?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}

namespace eMechanic.Application.Abstractions.UserRepairPreferences;

using Domain.UserRepairPreferences;
using Repositories;

public interface IUserRepairPreferencesRepository : IRepository<UserRepairPreferences>
{
    Task<UserRepairPreferences?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}

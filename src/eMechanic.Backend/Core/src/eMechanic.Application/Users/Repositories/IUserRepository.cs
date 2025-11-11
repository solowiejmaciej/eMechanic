namespace eMechanic.Application.Users.Repositories;

using eMechanic.Application.Abstractions.Repositories;
using eMechanic.Domain.User;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByIdentityIdAsync(Guid identityId);
}

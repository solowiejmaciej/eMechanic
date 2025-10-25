namespace eMechanic.Application.Abstractions.User;

using Domain.User;
using Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByIdentityIdAsync(Guid identityId);
}

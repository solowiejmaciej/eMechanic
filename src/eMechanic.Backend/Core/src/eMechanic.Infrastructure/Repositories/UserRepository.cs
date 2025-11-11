namespace eMechanic.Infrastructure.Repositories;

using Application.Users.Repositories;
using Base;
using DAL;
using Domain.User;
using Microsoft.EntityFrameworkCore;
using Services;

internal sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context, IPaginationService paginationService) : base(context, paginationService)
    {
    }

    public Task<User?> GetByIdentityIdAsync(Guid identityId) => DbSet.SingleOrDefaultAsync(x => x.IdentityId == identityId);
}

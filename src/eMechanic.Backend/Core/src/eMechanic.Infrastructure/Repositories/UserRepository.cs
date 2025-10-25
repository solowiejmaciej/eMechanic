namespace eMechanic.Infrastructure.Repositories;

using Application.Abstractions.User;
using Base;
using DAL;
using Domain.User;
using Services;

internal sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context, IPaginationService paginationService) : base(context, paginationService)
    {
    }
}

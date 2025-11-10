namespace eMechanic.Infrastructure.Repositories;

using Application.Abstractions.UserRepairPreferences;
using Base;
using DAL;
using Domain.UserRepairPreferences;
using Extensions;
using Microsoft.EntityFrameworkCore;
using Services;

internal sealed class UserRepairPreferencesRepositoryRepository : Repository<UserRepairPreferences>, IUserRepairPreferencesRepository
{
    public UserRepairPreferencesRepositoryRepository(AppDbContext context, IPaginationService paginationService)
        : base(context, paginationService)
    {
    }

    public Task<UserRepairPreferences?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return GetQuery()
            .FilterByUserId(userId)
            .SingleOrDefaultAsync(cancellationToken);
    }
}

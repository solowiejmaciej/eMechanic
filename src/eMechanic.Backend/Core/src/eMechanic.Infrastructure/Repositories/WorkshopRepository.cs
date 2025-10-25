namespace eMechanic.Infrastructure.Repositories;

using Application.Abstractions.Workshop;
using Base;
using DAL;
using Domain.Workshop;
using Extensions;
using Microsoft.EntityFrameworkCore;
using Services;

public class WorkshopRepository : Repository<Workshop>, IWorkshopRepository
{
    public WorkshopRepository(AppDbContext context, IPaginationService paginationService) : base(context, paginationService)
    {
    }

    public Task<Workshop?> GetByIdentityIdAsync(Guid identityId)
    {
        return GetQuery()
            .FilterByIdentityId(identityId)
            .SingleOrDefaultAsync();
    }

}

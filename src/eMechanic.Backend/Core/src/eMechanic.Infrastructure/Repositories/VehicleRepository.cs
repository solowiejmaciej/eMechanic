namespace eMechanic.Infrastructure.Repositories;

using Application.Abstractions.Vehicle;
using Base;
using DAL;
using Domain.Vehicle;
using Extensions;
using Microsoft.EntityFrameworkCore;
using Services;

internal sealed class VehicleRepository : Repository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(AppDbContext context, IPaginationService paginationService) : base(context, paginationService)
    {
    }


    public async Task<Vehicle?> GetForUserById(Guid entityId, Guid userId, CancellationToken cancellationToken)
    {
        return await GetQuery()
            .FilterById(entityId)
            .FilterByUserId(userId)
            .SingleOrDefaultAsync(cancellationToken);
    }
}

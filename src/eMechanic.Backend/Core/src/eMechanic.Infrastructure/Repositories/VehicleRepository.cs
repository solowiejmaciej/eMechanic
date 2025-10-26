namespace eMechanic.Infrastructure.Repositories;

using Application.Abstractions.Vehicle;
using Base;
using Common.Result;
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


    public async Task<Vehicle?> GetForUserById(
        Guid entityId,
        Guid userId,
        CancellationToken cancellationToken)
        => await GetQuery()
            .FilterById(entityId)
            .FilterByUserId(userId)
            .SingleOrDefaultAsync(cancellationToken);

    public Task<PaginationResult<Vehicle>> GetForUserPaginatedAsync(PaginationParameters paginationParameters,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var query = GetQuery()
            .FilterByUserId(userId);

        return GetPaginatedAsync(query, paginationParameters, cancellationToken);
    }
}

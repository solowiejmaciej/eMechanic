namespace eMechanic.Infrastructure.Repositories;

using Application.Repair.Repositories;
using Base;
using Common.Result;
using DAL;
using Microsoft.EntityFrameworkCore;
using Repositories.Extensions;
using Services;
using RepairAggregate = eMechanic.Domain.Repair.Repair;

internal sealed class RepairRepository : Repository<RepairAggregate>, IRepairRepository
{
    private readonly AppDbContext _context;

    public RepairRepository(AppDbContext context, IPaginationService paginationService)
        : base(context, paginationService)
    {
        _context = context;
    }

    public Task<RepairAggregate?> GetForWorkshopByIdAsNoTrackingAsync(Guid workshopId, Guid repairId, CancellationToken cancellationToken)
    {
        var query = GetQuery()
            .AsNoTracking()
            .FilterByWorkshopId(workshopId)
            .FilterById(repairId);

        return query.SingleOrDefaultAsync(cancellationToken);
    }

    public Task<RepairAggregate?> GetForUserByIdAsNoTrackingAsync(Guid userId, Guid requestRepairId,
        CancellationToken cancellationToken)
    {
        var query = GetForUserAsNoTrackingQuery(userId)
            .Where(repair => repair.Id == requestRepairId);

        return query.SingleOrDefaultAsync(cancellationToken);
    }

    public Task<PaginationResult<RepairAggregate>> GetForUserPaginatedAsync(
        Guid userId,
        PaginationParameters paginationParameters,
        CancellationToken cancellationToken)
    {
        var query = GetForUserAsNoTrackingQuery(userId)
            .OrderByDescending(x => x.CreatedAt);

        return GetPaginatedAsync(query, paginationParameters, cancellationToken);
    }

    public Task<PaginationResult<RepairAggregate>> GetForWorkshopPaginatedAsync(
        Guid workshopId,
        PaginationParameters paginationParameters,
        CancellationToken cancellationToken)
    {
        var query = GetQuery()
            .AsNoTracking()
            .FilterByWorkshopId(workshopId);

        query = query.OrderByDescending(x => x.CreatedAt);

        return GetPaginatedAsync(query, paginationParameters, cancellationToken);
    }

    private IQueryable<RepairAggregate> GetForUserAsNoTrackingQuery(Guid userId)
    {
        return GetQuery()
            .AsNoTracking()
            .Join(
                _context.Vehicles.AsNoTracking().Where(vehicle => vehicle.UserId == userId),
                repair => repair.VehicleId,
                vehicle => vehicle.Id,
                (repair, _) => repair);
    }
}

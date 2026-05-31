namespace eMechanic.Infrastructure.Repositories;

using Application.Repair.Repositories;
using Base;
using DAL;
using Microsoft.EntityFrameworkCore;
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
            .Where(repair => repair.Id == repairId && repair.WorkshopId == workshopId);

        return query.SingleOrDefaultAsync(cancellationToken);
    }

    public Task<RepairAggregate?> GetForUserByIdAsNoTrackingAsync(Guid userId, Guid repairId, CancellationToken cancellationToken)
    {
        var query =
            from repair in GetQuery().AsNoTracking()
            join vehicle in _context.Vehicles.AsNoTracking() on repair.VehicleId equals vehicle.Id
            where repair.Id == repairId && vehicle.UserId == userId
            select repair;

        return query.SingleOrDefaultAsync(cancellationToken);
    }
}


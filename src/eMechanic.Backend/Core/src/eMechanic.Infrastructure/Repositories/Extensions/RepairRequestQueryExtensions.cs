
namespace eMechanic.Infrastructure.Repositories.Extensions;

using System.Linq.Expressions;
using Domain.RepairRequest;

internal static class RepairRequestQueryExtensions
{
    public static IQueryable<RepairRequest> FilterByUserId(this IQueryable<RepairRequest> query, Guid userId)
    {
        return query.Where(rr => rr.UserId == userId);
    }

    public static IQueryable<RepairRequest> FilterByWorkshopId(this IQueryable<RepairRequest> query, Guid workshopId)
    {
        return query.Where(rr => rr.WorkshopId == workshopId);
    }
    
    public static IQueryable<RepairRequest> FilterByVehicleId(this IQueryable<RepairRequest> query, Guid vehicleId)
    {
        return query.Where(rr => rr.VehicleId == vehicleId);
    }
}

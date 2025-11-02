namespace eMechanic.Infrastructure.Repositories.Extensions;

using Domain.References.Vehicle;

public static class VehicleReferencedExtensions
{
    public static IQueryable<T> FilterByVehicleId<T>(this IQueryable<T> query, Guid vehicleId)
        where T : IVehicleReference
    {
        query = query.Where(u => u.VehicleId == vehicleId);
        return query;
    }
}

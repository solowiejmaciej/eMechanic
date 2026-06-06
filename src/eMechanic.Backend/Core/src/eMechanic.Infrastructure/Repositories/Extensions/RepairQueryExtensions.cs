namespace eMechanic.Infrastructure.Repositories.Extensions;

using Domain.Repair;

internal static class RepairQueryExtensions
{
    public static IQueryable<Repair> FilterByWorkshopId(this IQueryable<Repair> query, Guid workshopId)
    {
        return query.Where(r => r.WorkshopId == workshopId);
    }
}


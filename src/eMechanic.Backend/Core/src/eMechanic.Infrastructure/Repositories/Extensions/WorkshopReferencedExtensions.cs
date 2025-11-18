namespace eMechanic.Infrastructure.Repositories.Extensions;

using Domain.Shared.References.Workshop;

public static class WorkshopReferencedExtensions
{
    public static IQueryable<T> FilterByWorkshopId<T>(this IQueryable<T> query, Guid workshopId)
        where T : IWorkshopReference
    {
        query = query.Where(u => u.WorkshopId == workshopId);
        return query;
    }
}

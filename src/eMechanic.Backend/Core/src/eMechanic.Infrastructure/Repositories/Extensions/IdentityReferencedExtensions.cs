namespace eMechanic.Infrastructure.Repositories.Extensions;

using Domain.References.Identity;

public static class IdentityReferencedExtensions
{
    public static IQueryable<T> FilterByIdentityId<T>(this IQueryable<T> query, Guid identityId)
        where T : IIdentityReference
    {
        query = query.Where(u => u.IdentityId == identityId);
        return query;
    }
}

namespace eMechanic.Infrastructure.Repositories.Extensions;

using Common.DDD;

public static class EntityExtensionsExtensions
{
    public static IQueryable<T> FilterById<T>(this IQueryable<T> query, Guid Id)
        where T : Entity
    {
        query = query.Where(x => x.Id == Id);
        return query;
    }

    public static IQueryable<T> FilterByCreatedAt<T>(this IQueryable<T> query, DateTime? from, DateTime? to)
        where T : Entity
    {
        if (from.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.CreatedAt <= to.Value);
        }

        return query;
    }
}


namespace eMechanic.Infrastructure.Repositories.Extensions;

using Domain.Shared.References.User;

public static class UserReferencedExtensions
{
    public static IQueryable<T> FilterByUserId<T>(this IQueryable<T> query, Guid userId)
        where T : IUserReferenced
    {
        query = query.Where(u => u.UserId == userId);
        return query;
    }
}

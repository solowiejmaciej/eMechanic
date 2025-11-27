namespace eMechanic.Infrastructure.Repositories.Extensions;

using System.Linq.Expressions;
using System.Reflection;
using Common.Attributes;
using Common.DDD;

public static class EntityExtensions
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

    public static IQueryable<T> ApplySearch<T>(this IQueryable<T> source, string? searchPhrase)
    {
        if (string.IsNullOrWhiteSpace(searchPhrase))
        {
            return source;
        }

        var properties = typeof(T).GetProperties()
            .Where(p => p.GetCustomAttribute<SearchableAttribute>() != null && p.PropertyType == typeof(string))
            .ToList();

        if (properties.Count == 0)
        {
            return source;
        }

        var parameter = Expression.Parameter(typeof(T), "x");
        var searchExpression = Expression.Constant(searchPhrase.Trim().ToLower(System.Globalization.CultureInfo.InvariantCulture));

        var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
        var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);

        Expression? predicateBody = null;

        foreach (var property in properties)
        {
            var propertyAccess = Expression.Property(parameter, property);

            var toLowerExpression = Expression.Call(propertyAccess, toLowerMethod!);

            var containsExpression = Expression.Call(toLowerExpression, containsMethod!, searchExpression);

            if (predicateBody == null)
            {
                predicateBody = containsExpression;
            }
            else
            {
                predicateBody = Expression.OrElse(predicateBody, containsExpression);
            }
        }

        if (predicateBody == null)
        {
            return source;
        }

        var lambda = Expression.Lambda<Func<T, bool>>(predicateBody, parameter);

        return source.Where(lambda);
    }
}


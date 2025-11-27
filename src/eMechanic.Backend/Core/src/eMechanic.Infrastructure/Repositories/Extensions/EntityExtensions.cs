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

        var searchableProperties = typeof(T).GetProperties()
            .Where(p => p.GetCustomAttribute<SearchableAttribute>() != null)
            .ToList();

        if (searchableProperties.Count == 0)
        {
            return source;
        }

        var parameter = Expression.Parameter(typeof(T), "x");
        var searchConstant = Expression.Constant(searchPhrase.Trim().ToLower(System.Globalization.CultureInfo.InvariantCulture));

        var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
        var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);

        Expression? combinedExpression = null;

        foreach (var property in searchableProperties)
        {
            Expression propertyAccess = Expression.Property(parameter, property);
            Expression? searchClause = null;

            if (property.PropertyType == typeof(string))
            {
                var notNullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null, typeof(string)));
                var toLowerCall = Expression.Call(propertyAccess, toLowerMethod!);
                var containsCall = Expression.Call(toLowerCall, containsMethod!, searchConstant);
                searchClause = Expression.AndAlso(notNullCheck, containsCall);
            }
            else
            {
                var valueProperty = property.PropertyType.GetProperty("Value");
                if (valueProperty?.PropertyType == typeof(string))
                {
                    var propertyNotNull = Expression.NotEqual(propertyAccess, Expression.Constant(null, property.PropertyType));
                    var valuePropertyAccess = Expression.Property(propertyAccess, valueProperty);
                    var valueNotNull = Expression.NotEqual(valuePropertyAccess, Expression.Constant(null, typeof(string)));
                    var toLowerCall = Expression.Call(valuePropertyAccess, toLowerMethod!);
                    var containsCall = Expression.Call(toLowerCall, containsMethod!, searchConstant);

                    var valueChecks = Expression.AndAlso(valueNotNull, containsCall);
                    searchClause = Expression.AndAlso(propertyNotNull, valueChecks);
                }
            }

            if (searchClause != null)
            {
                combinedExpression = combinedExpression == null
                    ? searchClause
                    : Expression.OrElse(combinedExpression, searchClause);
            }
        }

        if (combinedExpression == null)
        {
            return source;
        }

        var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
        return source.Where(lambda);
    }
}


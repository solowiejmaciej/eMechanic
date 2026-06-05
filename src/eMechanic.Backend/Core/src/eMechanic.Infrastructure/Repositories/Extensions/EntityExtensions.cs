namespace eMechanic.Infrastructure.Repositories.Extensions;

using System.Linq.Expressions;
using System.Reflection;
using Common.Attributes;
using Common.DDD;
using Microsoft.EntityFrameworkCore;

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
            var propertyAccess = Expression.Property(parameter, property);
            var searchClause = BuildSearchClause(parameter, propertyAccess, property, searchConstant, containsMethod!, toLowerMethod!);

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

    private static BinaryExpression? BuildSearchClause(
        ParameterExpression entityParameter,
        Expression propertyAccess,
        PropertyInfo property,
        Expression searchConstant,
        MethodInfo containsMethod,
        MethodInfo toLowerMethod)
    {
        if (property.PropertyType == typeof(string))
        {
            return BuildStringContainsClause(propertyAccess, searchConstant, containsMethod, toLowerMethod);
        }

        var valueProperty = property.PropertyType.GetProperty("Value");
        if (valueProperty?.PropertyType == typeof(string))
        {
            var propertyNotNull = Expression.NotEqual(propertyAccess, Expression.Constant(null, property.PropertyType));
            var valuePropertyAccess = Expression.Property(propertyAccess, valueProperty);
            var containsClause = BuildStringContainsClause(valuePropertyAccess, searchConstant, containsMethod, toLowerMethod);
            return Expression.AndAlso(propertyNotNull, containsClause);
        }

        // Fallback for scalar value objects persisted with HasConversion(... -> string).
        return BuildEfPropertyStringClause(entityParameter, property.Name, searchConstant, containsMethod, toLowerMethod);
    }

    private static BinaryExpression BuildStringContainsClause(
        Expression valueExpression,
        Expression searchConstant,
        MethodInfo containsMethod,
        MethodInfo toLowerMethod)
    {
        var valueNotNull = Expression.NotEqual(valueExpression, Expression.Constant(null, typeof(string)));
        var toLowerCall = Expression.Call(valueExpression, toLowerMethod);
        var containsCall = Expression.Call(toLowerCall, containsMethod, searchConstant);

        return Expression.AndAlso(valueNotNull, containsCall);
    }

    private static BinaryExpression BuildEfPropertyStringClause(
        ParameterExpression entityParameter,
        string propertyName,
        Expression searchConstant,
        MethodInfo containsMethod,
        MethodInfo toLowerMethod)
    {
        var efPropertyMethod = typeof(EF)
            .GetMethod(nameof(EF.Property), BindingFlags.Public | BindingFlags.Static)!
            .MakeGenericMethod(typeof(string));

        var valueExpression = Expression.Call(efPropertyMethod, entityParameter, Expression.Constant(propertyName));
        return BuildStringContainsClause(valueExpression, searchConstant, containsMethod, toLowerMethod);
    }
}

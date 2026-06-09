namespace eMechanic.Infrastructure.Repositories.Extensions;

using System.Linq.Expressions;
using System.Reflection;
using System.Globalization;
using Common.Attributes;
using Common.DDD;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

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

        var trimmedSearchPhrase = searchPhrase.Trim();

        var isEfQueryProvider = source.Provider is IAsyncQueryProvider;

        var searchableProperties = typeof(T).GetProperties()
            .Where(p => p.GetCustomAttribute<SearchableAttribute>() != null)
            .ToList();

        if (searchableProperties.Count == 0)
        {
            return source;
        }

        var parameter = Expression.Parameter(typeof(T), "x");
        var searchConstant = Expression.Constant(trimmedSearchPhrase.ToLower(System.Globalization.CultureInfo.InvariantCulture));

        var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
        var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);

        Expression? combinedExpression = null;

        foreach (var property in searchableProperties)
        {
            var propertyAccess = Expression.Property(parameter, property);
            var searchClause = BuildSearchClause(
                parameter,
                propertyAccess,
                property,
                searchConstant,
                trimmedSearchPhrase,
                isEfQueryProvider,
                containsMethod!,
                toLowerMethod!);

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
        string rawSearchPhrase,
        bool isEfQueryProvider,
        MethodInfo containsMethod,
        MethodInfo toLowerMethod)
    {
        if (property.PropertyType.IsEnum)
        {
            // For EF queries: x.Status == ERepairRequestStatus.Pending is fully translatable
            // when the enum is mapped with HasConversion<string>(). EF applies the converter
            // to the constant and generates: "Status" = 'Pending'. No EF.Property needed.
            return BuildEnumEqualsClause(propertyAccess, property.PropertyType, rawSearchPhrase);
        }

        var scalarClause = BuildScalarEqualsClause(propertyAccess, property.PropertyType, rawSearchPhrase);
        if (scalarClause is not null)
        {
            return scalarClause;
        }

        if (property.PropertyType == typeof(string))
        {
            return BuildStringContainsClause(propertyAccess, searchConstant, containsMethod, toLowerMethod);
        }

        var valueProperty = property.PropertyType.GetProperty("Value");
        if (valueProperty?.PropertyType == typeof(string))
        {
            // For OwnsOne-mapped VOs, EF Core can translate x.Prop.Value as a column access.
            // All [Searchable] string VOs must be mapped with OwnsOne (not HasConversion) to
            // keep this expression fully EF-translatable without value-converter cast issues.
            if (property.PropertyType.IsValueType)
            {
                // Struct VO — no null check needed
                var valuePropertyAccess = Expression.Property(propertyAccess, valueProperty);
                return BuildStringContainsClause(valuePropertyAccess, searchConstant, containsMethod, toLowerMethod);
            }

            var propertyNotNull = Expression.NotEqual(propertyAccess, Expression.Constant(null, property.PropertyType));
            var valuePropertyAccessRef = Expression.Property(propertyAccess, valueProperty);
            var containsClause = BuildStringContainsClause(valuePropertyAccessRef, searchConstant, containsMethod, toLowerMethod);
            return Expression.AndAlso(propertyNotNull, containsClause);
        }

        if (valueProperty is not null)
        {
            var valuePropertyAccess = Expression.Property(propertyAccess, valueProperty);
            var valueClause = BuildScalarEqualsClause(valuePropertyAccess, valueProperty.PropertyType, rawSearchPhrase);

            if (valueClause is not null)
            {
                if (!property.PropertyType.IsValueType)
                {
                    var propertyNotNull = Expression.NotEqual(propertyAccess, Expression.Constant(null, property.PropertyType));
                    return Expression.AndAlso(propertyNotNull, valueClause);
                }

                return valueClause;
            }

            return null;
        }

        return BuildEfPropertyStringClause(entityParameter, property.Name, searchConstant, containsMethod, toLowerMethod);
    }

    private static BinaryExpression? BuildEnumEqualsClause(
        Expression propertyAccess,
        Type enumType,
        string rawSearchPhrase)
    {
        if (!Enum.TryParse(enumType, rawSearchPhrase, true, out var parsedValue) || parsedValue is null)
        {
            return null;
        }

        var enumConstant = Expression.Constant(parsedValue, enumType);
        return Expression.Equal(propertyAccess, enumConstant);
    }

    private static BinaryExpression? BuildEfEnumEqualsClause(
        ParameterExpression entityParameter,
        Type enumType,
        string propertyName,
        string rawSearchPhrase)
    {
        if (!Enum.TryParse(enumType, rawSearchPhrase, true, out var parsedValue) || parsedValue is null)
        {
            return null;
        }

        var efPropertyMethod = typeof(EF)
            .GetMethod(nameof(EF.Property), BindingFlags.Public | BindingFlags.Static)!
            .MakeGenericMethod(typeof(string));

        var valueExpression = Expression.Call(efPropertyMethod, entityParameter, Expression.Constant(propertyName));
        var enumName = Expression.Constant(parsedValue.ToString(), typeof(string));

        return Expression.Equal(valueExpression, enumName);
    }

    private static BinaryExpression? BuildScalarEqualsClause(
        Expression propertyAccess,
        Type propertyType,
        string rawSearchPhrase)
    {
        if (!TryParseSearchValue(rawSearchPhrase, propertyType, out var parsedValue))
        {
            return null;
        }

        var constantType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        Expression constantExpression = Expression.Constant(parsedValue, constantType);

        if (constantType != propertyType)
        {
            constantExpression = Expression.Convert(constantExpression, propertyType);
        }

        return Expression.Equal(propertyAccess, constantExpression);
    }

    private static bool TryParseSearchValue(string rawSearchPhrase, Type targetType, out object? parsedValue)
    {
        var type = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (type == typeof(byte))
        {
            var success = byte.TryParse(rawSearchPhrase, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value);
            parsedValue = value;
            return success;
        }

        if (type == typeof(short))
        {
            var success = short.TryParse(rawSearchPhrase, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value);
            parsedValue = value;
            return success;
        }

        if (type == typeof(int))
        {
            var success = int.TryParse(rawSearchPhrase, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value);
            parsedValue = value;
            return success;
        }

        if (type == typeof(long))
        {
            var success = long.TryParse(rawSearchPhrase, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value);
            parsedValue = value;
            return success;
        }

        if (type == typeof(decimal))
        {
            var success = decimal.TryParse(rawSearchPhrase, NumberStyles.Number, CultureInfo.InvariantCulture, out var value);
            parsedValue = value;
            return success;
        }

        if (type == typeof(double))
        {
            var success = double.TryParse(rawSearchPhrase, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var value);
            parsedValue = value;
            return success;
        }

        if (type == typeof(float))
        {
            var success = float.TryParse(rawSearchPhrase, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var value);
            parsedValue = value;
            return success;
        }

        if (type == typeof(Guid))
        {
            var success = Guid.TryParse(rawSearchPhrase, out var value);
            parsedValue = value;
            return success;
        }

        if (type == typeof(bool))
        {
            var success = bool.TryParse(rawSearchPhrase, out var value);
            parsedValue = value;
            return success;
        }

        parsedValue = null;
        return false;
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

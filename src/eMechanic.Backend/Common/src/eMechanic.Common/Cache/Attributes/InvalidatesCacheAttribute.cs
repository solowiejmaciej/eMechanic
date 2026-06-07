namespace eMechanic.Common.Cache.Attributes;

using System;

/// <summary>
/// Marks a command as invalidating one or more query caches upon successful execution.
/// The cache group version for each listed query type is bumped, causing subsequent reads to miss cache
/// and fetch fresh data. Old entries are orphaned and expire naturally via their own TTL.
/// </summary>
/// <example>
/// [InvalidatesCache(typeof(GetWorkshopReviewsQuery), typeof(GetWorkshopReviewStatsQuery))]
/// public sealed record UpsertWorkshopReviewCommand(...) : IResultCommand&lt;Guid&gt;;
/// </example>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public sealed class InvalidatesCacheAttribute : Attribute
{
    public Type[] QueryTypes { get; }

    public InvalidatesCacheAttribute(params Type[] queryTypes)
    {
        QueryTypes = queryTypes ?? throw new ArgumentNullException(nameof(queryTypes));
    }
}



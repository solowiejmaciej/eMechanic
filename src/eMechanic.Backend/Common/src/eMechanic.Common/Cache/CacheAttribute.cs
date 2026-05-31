namespace eMechanic.Common.Cache;

using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public sealed class CacheAttribute : Attribute
{
    public int DurationSeconds { get; }
    public ECacheScope Scope { get; }

    public CacheAttribute(int durationSeconds, ECacheScope scope)
    {
        DurationSeconds = durationSeconds;
        Scope = scope;
    }
}

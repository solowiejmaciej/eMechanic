namespace eMechanic.Common.Cache;

using System;

public sealed class CacheRule<TRequest>
{
    public TimeSpan Expiration { get; }
    public ECacheScope Scope { get; }
    public string Key { get; }

    public CacheRule(TimeSpan expiration, ECacheScope scope, string key)
    {
        Expiration = expiration;
        Scope = scope;
        Key = key;
    }

    public CacheRule(TimeSpan expiration, ECacheScope scope)
    {
        Expiration = expiration;
        Scope = scope;
        Key = typeof(TRequest).Name;
    }
}

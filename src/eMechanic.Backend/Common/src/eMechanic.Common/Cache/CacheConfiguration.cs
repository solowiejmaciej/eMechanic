namespace eMechanic.Common.Cache;

using System;
using System.Collections.Concurrent;

public sealed class CacheConfiguration : ICacheConfiguration
{
    private readonly ConcurrentDictionary<Type, object> _rules = new();

    public bool TryGetRule(Type requestType, out object? rule) => _rules.TryGetValue(requestType, out rule);
    public void Register<TRequest>(CacheRule<TRequest> rule) => _rules[typeof(TRequest)] = rule;
}

public interface ICacheConfiguration
{
    bool TryGetRule(Type requestType, out object? rule);
    void Register<TRequest>(CacheRule<TRequest> rule);
}

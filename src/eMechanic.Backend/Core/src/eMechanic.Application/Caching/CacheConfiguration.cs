namespace eMechanic.Application.Caching;

using System;
using System.Collections.Concurrent;

public sealed class CacheConfiguration : ICacheConfiguration
{
    private readonly ConcurrentDictionary<Type, object> _rules = new();

    public void Register<TRequest>(CacheRule<TRequest> rule) => _rules[typeof(TRequest)] = rule ?? throw new ArgumentNullException(nameof(rule));

    public bool TryGetRule(Type requestType, out object? rule) => _rules.TryGetValue(requestType, out rule);
}

public interface ICacheConfiguration
{
    bool TryGetRule(Type requestType, out object? rule);
}

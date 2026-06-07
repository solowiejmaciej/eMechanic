namespace eMechanic.Common.Cache.Configuration;

using System;
using System.Collections.Concurrent;

public sealed class CacheConfiguration : ICacheConfiguration
{
    private readonly ConcurrentDictionary<Type, object> _rules = new();
    private readonly ConcurrentDictionary<Type, Type[]> _invalidations = new();

    public bool TryGetRule(Type requestType, out object? rule) => _rules.TryGetValue(requestType, out rule);
    public void Register<TRequest>(CacheRule<TRequest> rule) => _rules[typeof(TRequest)] = rule;

    public void RegisterInvalidation(Type commandType, Type[] queryTypes) =>
        _invalidations[commandType] = queryTypes;

    public bool TryGetInvalidationTargets(Type commandType, out Type[]? targets) =>
        _invalidations.TryGetValue(commandType, out targets);
}

public interface ICacheConfiguration
{
    bool TryGetRule(Type requestType, out object? rule);
    void Register<TRequest>(CacheRule<TRequest> rule);

    /// <summary>Registers which query cache groups a command type should invalidate on success.</summary>
    void RegisterInvalidation(Type commandType, Type[] queryTypes);

    /// <summary>Returns the query types whose cache groups should be bumped when <paramref name="commandType"/> succeeds.</summary>
    bool TryGetInvalidationTargets(Type commandType, out Type[]? targets);
}


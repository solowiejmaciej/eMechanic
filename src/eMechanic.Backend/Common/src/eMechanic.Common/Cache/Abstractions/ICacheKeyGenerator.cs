namespace eMechanic.Common.Cache.Abstractions;

using Configuration;

public interface ICacheKeyGenerator
{
    string GenerateKey<TRequest>(CacheRule<TRequest> rule, TRequest request);
}



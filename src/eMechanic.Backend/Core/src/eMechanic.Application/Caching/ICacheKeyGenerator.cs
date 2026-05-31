namespace eMechanic.Application.Caching;

using Common.Cache;

public interface ICacheKeyGenerator
{
    string GenerateKey<TRequest>(CacheRule<TRequest> rule, TRequest request);
}

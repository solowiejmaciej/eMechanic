namespace eMechanic.Application.Caching;

public sealed class CacheRule<TRequest>
{
    public TimeSpan Expiration { get; }
    public Func<TRequest, string> KeyFactory { get; }

    public CacheRule(TimeSpan expiration, Func<TRequest, string> keyFactory)
    {
        Expiration = expiration;
        KeyFactory = keyFactory ?? throw new ArgumentNullException(nameof(keyFactory));
    }
}

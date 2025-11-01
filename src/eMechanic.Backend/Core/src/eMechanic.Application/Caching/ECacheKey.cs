namespace eMechanic.Application.Caching;

public enum ECacheKey
{
    None = 0,
    GetUserById = 1
}

public static class CacheKeyExtensions
{
    // public static string ToCacheKeyString(this ECacheKey key, Guid id) => key switch
    // {
    //     ECacheKey.GetUserById => $"{nameof(ECacheKey.GetUserById)}-{id}",
    //     _ => throw new ArgumentOutOfRangeException(nameof(key), key, null)
    // };
}

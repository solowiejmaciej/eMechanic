namespace eMechanic.Common.Cache.Services;

using Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

internal static class CacheInvalidationDefaults
{
	// Group version keys live for 7 days - much longer than any individual entry TTL.
	internal static readonly DistributedCacheEntryOptions GroupVersionOptions = new()
	{
		AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
	};
}

public sealed class CacheInvalidationService : ICacheInvalidationService
{
	private readonly IDistributedCache _cache;
	private readonly ILogger<CacheInvalidationService> _logger;

	public CacheInvalidationService(
		IDistributedCache cache,
		ILogger<CacheInvalidationService> logger)
	{
		_cache = cache;
		_logger = logger;
	}

	public async Task InvalidateAsync(Type commandType, IReadOnlyCollection<Type> queryTypes, CancellationToken cancellationToken)
	{
		foreach (var queryType in queryTypes)
		{
			var groupKey = $"cacheGroup:{queryType.Name}";
			var newVersion = Guid.NewGuid().ToString("N");

			try
			{
				await _cache.SetStringAsync(groupKey, newVersion, CacheInvalidationDefaults.GroupVersionOptions, cancellationToken);
				_logger.LogInformation(
					"Cache group '{GroupKey}' invalidated (new version: {Version}) after {CommandType} succeeded.",
					groupKey, newVersion, commandType.Name);
			}
			catch (Exception ex)
			{
				// Invalidation failure must never crash the command - log and continue.
				_logger.LogWarning(ex,
					"Failed to invalidate cache group '{GroupKey}' after {CommandType}.",
					groupKey, commandType.Name);
			}
		}
	}
}


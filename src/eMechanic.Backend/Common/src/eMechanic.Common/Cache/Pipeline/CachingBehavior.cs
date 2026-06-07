namespace eMechanic.Common.Cache.Pipeline;

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Abstractions;
using Configuration;
using eMechanic.Common.CQRS;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

public sealed class CachingBehavior<TRequest, TResponse> : IResultPipelineBehavior<TRequest, TResponse>
{
	private readonly IDistributedCache _cache;
	private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;
	private readonly ICacheConfiguration _config;
	private readonly ICacheKeyGenerator _cacheKeyGenerator;

	public CachingBehavior(
		IDistributedCache cache,
		ILogger<CachingBehavior<TRequest, TResponse>> logger,
		ICacheConfiguration config,
		ICacheKeyGenerator cacheKeyGenerator)
	{
		_cache = cache;
		_logger = logger;
		_config = config;
		_cacheKeyGenerator = cacheKeyGenerator;
	}

	private static string GroupVersionKey => $"cacheGroup:{typeof(TRequest).Name}";

	public async Task<TResponse> Handle(
		TRequest request,
		Func<CancellationToken, Task<TResponse>> next,
		CancellationToken cancellationToken)
	{
		if (!_config.TryGetRule(typeof(TRequest), out var boxedRule))
		{
			return await next(cancellationToken);
		}

		if (boxedRule is not CacheRule<TRequest> rule)
		{
			_logger.LogWarning("Cache rule for {RequestType} has wrong type.", typeof(TRequest).Name);
			return await next(cancellationToken);
		}

		string groupVersion;
		try
		{
			groupVersion = await _cache.GetStringAsync(GroupVersionKey, cancellationToken) ?? "0";
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Failed to read cache group version for {RequestType}.", typeof(TRequest).Name);
			groupVersion = "0";
		}

		var cacheKey = $"{_cacheKeyGenerator.GenerateKey(rule, request)}:{groupVersion}";

		string? cachedValue;
		try
		{
			cachedValue = await _cache.GetStringAsync(cacheKey, cancellationToken);
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Failed to read from cache for key {CacheKey}.", cacheKey);
			cachedValue = null;
		}

		if (cachedValue is not null)
		{
			_logger.LogInformation("Cache HIT for key: {CacheKey}", cacheKey);
			try
			{
				var cachedResult = JsonSerializer.Deserialize<TResponse>(cachedValue);
				if (cachedResult is not null)
				{
					return cachedResult;
				}
				_logger.LogWarning("Failed to deserialize cached value for key {CacheKey}.", cacheKey);
			}
			catch (JsonException ex)
			{
				_logger.LogWarning(ex, "Failed to deserialize cached value for key {CacheKey}.", cacheKey);
			}
		}

		_logger.LogInformation("Cache MISS for key: {CacheKey}", cacheKey);

		var response = await next(cancellationToken);

		if (!ResultErrorInspector<TResponse>.IsSuccessResponseType)
		{
			await TryWriteCache(cacheKey, response, rule.Expiration, cancellationToken);
			return response;
		}

		bool hasError = ResultErrorInspector<TResponse>.HasError(response);

		if (!hasError && response is not null)
		{
			await TryWriteCache(cacheKey, response, rule.Expiration, cancellationToken);
		}
		else
		{
			_logger.LogInformation("Response contains error, skipping cache write for key: {CacheKey}", cacheKey);
		}

		return response;
	}

	private async Task TryWriteCache(string key, TResponse response, TimeSpan expiration, CancellationToken cancellationToken)
	{
		try
		{
			var options = new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = expiration
			};
			var serializedResponse = JsonSerializer.Serialize(response);
			await _cache.SetStringAsync(key, serializedResponse, options, cancellationToken);
			_logger.LogInformation("Value cached successfully for key: {CacheKey}", key);
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Failed to write to cache for key: {CacheKey}", key);
		}
	}
}


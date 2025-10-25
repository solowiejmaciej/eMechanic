// csharp
namespace eMechanic.Application.Behaviors;

using System;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common.Result;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using eMechanic.Application.Caching;

public sealed class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;
    private readonly ICacheConfiguration _config;
    private static readonly MethodInfo? HasErrorMethod = typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<,>)
        ? typeof(TResponse).GetMethod("HasError", BindingFlags.Public | BindingFlags.Instance) ?? typeof(TResponse).GetProperty("HasError")?.GetGetMethod()
        : null;

    public CachingBehavior(
        IDistributedCache cache,
        ILogger<CachingBehavior<TRequest, TResponse>> logger,
        ICacheConfiguration config)
    {
        _cache = cache;
        _logger = logger;
        _config = config;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
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

        string cacheKey;
        try
        {
            cacheKey = rule.KeyFactory(request);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "KeyFactory threw for {RequestType}. Proceeding without cache.", typeof(TRequest).Name);
            return await next(cancellationToken);
        }

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
            _logger.LogDebug("Cache HIT for key: {CacheKey}", cacheKey);
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

        _logger.LogDebug("Cache MISS for key: {CacheKey}", cacheKey);

        var response = await next(cancellationToken);

        if (HasErrorMethod is null)
        {
            await TryWriteCache(cacheKey, response, rule.Expiration, cancellationToken);
            return response;
        }

        bool hasError = (bool)HasErrorMethod.Invoke(response, null)!;

        if (!hasError && response is not null)
        {
            await TryWriteCache(cacheKey, response, rule.Expiration, cancellationToken);
        }
        else
        {
            _logger.LogDebug("Response contains error, skipping cache write for key: {CacheKey}", cacheKey);
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
            _logger.LogDebug("Value cached successfully for key: {CacheKey}", key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to write to cache for key: {CacheKey}", key);
        }
    }
}

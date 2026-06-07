namespace eMechanic.Common.Cache.Pipeline;

using System.Threading;
using System.Threading.Tasks;
using Abstractions;
using Attributes;
using Configuration;
using eMechanic.Common.CQRS;
using Microsoft.Extensions.Logging;

/// <summary>
/// Pipeline behavior that invalidates cache groups after a command succeeds.
/// Attach <see cref="InvalidatesCacheAttribute"/> to a command record to declare which query
/// cache groups should be bumped.
/// </summary>
public sealed class CacheInvalidationBehavior<TRequest, TResponse> : IResultPipelineBehavior<TRequest, TResponse>
	where TRequest : notnull
{
	private readonly ICacheConfiguration _config;
	private readonly ICacheInvalidationService _cacheInvalidationService;
	private readonly ILogger<CacheInvalidationBehavior<TRequest, TResponse>> _logger;

	public CacheInvalidationBehavior(
		ICacheConfiguration config,
		ICacheInvalidationService cacheInvalidationService,
		ILogger<CacheInvalidationBehavior<TRequest, TResponse>> logger)
	{
		_config = config;
		_cacheInvalidationService = cacheInvalidationService;
		_logger = logger;
	}

	public async Task<TResponse> Handle(
		TRequest request,
		Func<CancellationToken, Task<TResponse>> next,
		CancellationToken cancellationToken)
	{
		if (!_config.TryGetInvalidationTargets(typeof(TRequest), out var targets) || targets is null || targets.Length == 0)
		{
			return await next(cancellationToken);
		}

		var response = await next(cancellationToken);

		if (ResultErrorInspector<TResponse>.IsSuccessResponseType && ResultErrorInspector<TResponse>.HasError(response))
		{
			_logger.LogInformation(
				"Skipping cache invalidation for {CommandType} because the result contained an error.",
				typeof(TRequest).Name);
			return response;
		}

		await _cacheInvalidationService.InvalidateAsync(typeof(TRequest), targets, cancellationToken);

		return response;
	}
}


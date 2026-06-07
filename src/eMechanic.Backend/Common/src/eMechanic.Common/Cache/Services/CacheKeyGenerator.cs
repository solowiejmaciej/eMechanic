namespace eMechanic.Common.Cache.Services;

using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Abstractions;
using Configuration;

public sealed class CacheKeyGenerator : ICacheKeyGenerator
{
	private readonly ICacheScopeContextAccessor? _scopeContextAccessor;

	public CacheKeyGenerator(ICacheScopeContextAccessor? scopeContextAccessor = null)
	{
		_scopeContextAccessor = scopeContextAccessor;
	}

	public string GenerateKey<TRequest>(CacheRule<TRequest> rule, TRequest request)
	{
		var basePart = GetDeterministicStateId(request);

		var typeSegment = typeof(TRequest).Name;

		return rule.Scope switch
		{
			ECacheScope.User => $"user:{_scopeContextAccessor?.GetUserIdOrDefault()}:{typeSegment}:{basePart}",
			ECacheScope.Workshop => $"workshop:{_scopeContextAccessor?.GetWorkshopIdOrDefault()}:{typeSegment}:{basePart}",
			ECacheScope.Public => $"public:{typeSegment}:{basePart}",
			_ => $"{typeSegment}:{basePart}"
		};
	}

	private static string GetDeterministicStateId(object? request)
	{
		if (request is null)
		{
			return "null";
		}

		var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		});

		byte[] inputBytes = Encoding.UTF8.GetBytes(json);
		byte[] hashBytes = SHA256.HashData(inputBytes);

		return Convert.ToHexString(hashBytes);
	}
}


namespace eMechanic.Application.Caching;

using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Common.Cache;
using eMechanic.Application.Abstractions.Identity.Contexts;

public sealed class CacheKeyGenerator : ICacheKeyGenerator
{
    private readonly IUserContext? _userContext;
    private readonly IWorkshopContext? _workshopContext;

    public CacheKeyGenerator(IUserContext? userContext = null, IWorkshopContext? workshopContext = null)
    {
        _userContext = userContext;
        _workshopContext = workshopContext;
    }

    public string GenerateKey<TRequest>(CacheRule<TRequest> rule, TRequest request)
    {
        var basePart = GetDeterministicStateId(request);

        var typeSegment = typeof(TRequest).Name;

        return rule.Scope switch
        {
            ECacheScope.User => $"user:{_userContext?.GetUserId()}:{typeSegment}:{basePart}",
            ECacheScope.Workshop => $"workshop:{_workshopContext?.GetWorkshopId()}:{typeSegment}:{basePart}",
            ECacheScope.Public => $"public:{typeSegment}:{basePart}",
            _ => $"{typeSegment}:{basePart}"
        };
    }

    private static string GetDeterministicStateId(object? request)
    {
        if (request is null) return "null";

        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        byte[] inputBytes = Encoding.UTF8.GetBytes(json);
        byte[] hashBytes = SHA256.HashData(inputBytes);

        return Convert.ToHexString(hashBytes);
    }
}

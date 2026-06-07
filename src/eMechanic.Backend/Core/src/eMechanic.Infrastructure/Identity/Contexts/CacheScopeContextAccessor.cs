namespace eMechanic.Infrastructure.Identity.Contexts;

using Common.Cache.Abstractions;
using Microsoft.AspNetCore.Http;

internal sealed class CacheScopeContextAccessor : ICacheScopeContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CacheScopeContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? GetUserIdOrDefault()
    {
        var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimConstants.USER_ID);
        return claim is not null && Guid.TryParse(claim.Value, out var userId) ? userId : null;
    }

    public Guid? GetWorkshopIdOrDefault()
    {
        var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimConstants.WORKSHOP_ID);
        return claim is not null && Guid.TryParse(claim.Value, out var workshopId) ? workshopId : null;
    }
}


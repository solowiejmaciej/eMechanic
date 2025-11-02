namespace eMechanic.Infrastructure.Identity.Contexts;

using eMechanic.Application.Abstractions.Identity.Contexts;
using Microsoft.AspNetCore.Http;

internal sealed class WorkshopContext : IWorkshopContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public WorkshopContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetWorkshopId()
    {
        if (!IsAuthenticated)
        {
            throw new UnauthorizedAccessException("Workshop is not authenticated.");
        }

        var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimConstants.WORKSHOP_ID);

        if (claim is not null && Guid.TryParse(claim.Value, out var workshopId))
        {
            return workshopId;
        }

        throw new UnauthorizedAccessException("WorkshopId claim is missing or invalid.");
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}

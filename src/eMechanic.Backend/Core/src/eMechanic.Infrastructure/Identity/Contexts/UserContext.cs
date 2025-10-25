namespace eMechanic.Infrastructure.Identity.Contexts;

using Application.Abstractions.Identity.Contexts;
using Microsoft.AspNetCore.Http;

internal sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            if (!IsAuthenticated)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimConstants.USER_ID);

            if (claim is not null && Guid.TryParse(claim.Value, out var userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("User ID claim is missing or invalid.");
        }
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}

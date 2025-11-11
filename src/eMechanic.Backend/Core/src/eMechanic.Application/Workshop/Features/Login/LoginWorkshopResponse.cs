namespace eMechanic.Application.Workshop.Features.Login;

public sealed record LoginWorkshopResponse(string Token, DateTime ExpiresAtUtc, Guid WorkshopId, string RefreshToken);

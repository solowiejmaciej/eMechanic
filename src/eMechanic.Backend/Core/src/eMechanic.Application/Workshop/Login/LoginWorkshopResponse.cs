namespace eMechanic.Application.Workshop.Login;

public sealed record LoginWorkshopResponse(string Token, DateTime ExpiresAtUtc, Guid WorkshopId);

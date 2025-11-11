namespace eMechanic.Application.Token.Features.Create.Workshop;

public sealed record CreateWorkshopTokenResponse(string Token, DateTime ExpiresAtUtc, Guid WorkshopId, string RefreshToken);

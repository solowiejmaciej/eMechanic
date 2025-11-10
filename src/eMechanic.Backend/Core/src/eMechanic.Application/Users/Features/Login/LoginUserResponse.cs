namespace eMechanic.Application.Users.Features.Login;

public sealed record LoginUserResponse(string Token, DateTime ExpiresAtUtc, Guid UserId);

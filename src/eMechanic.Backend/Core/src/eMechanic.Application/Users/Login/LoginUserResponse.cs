namespace eMechanic.Application.Users.Login;

public sealed record LoginUserResponse(string Token, DateTime ExpiresAtUtc, Guid UserId);

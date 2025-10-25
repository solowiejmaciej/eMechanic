namespace eMechanic.Application.Identity;

public record TokenDTO(string AccessToken, DateTime ExpiresAt);

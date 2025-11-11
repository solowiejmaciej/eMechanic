namespace eMechanic.Integration.Tests.Helpers;

public record FullAuthResponse(Guid DomainId, string Token, string RefreshToken);

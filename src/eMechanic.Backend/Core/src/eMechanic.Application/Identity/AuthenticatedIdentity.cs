namespace eMechanic.Application.Identity;

public record AuthenticatedIdentity(Guid IdentityId, Guid DomainEntityId, string Email, EIdentityType Type);

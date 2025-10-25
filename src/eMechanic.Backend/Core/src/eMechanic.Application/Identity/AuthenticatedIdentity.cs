namespace eMechanic.Application.Identity;

using Abstractions.Identity;

public record AuthenticatedIdentity(Guid IdentityId, Guid DomainEntityId, string Email, EIdentityType Type);

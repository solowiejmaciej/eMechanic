namespace eMechanic.Application.Identity;

using Abstractions.Identity;

public record AuthenticatedIdentity(Guid Id, string Email, EIdentityType Type);

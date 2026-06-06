namespace eMechanic.Domain.Shared.References.Identity;

using Shared.ValueObjects;

public interface IIdentityReference
{
    Guid IdentityId { get; }
    Email Email { get; }
}

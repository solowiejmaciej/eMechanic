namespace eMechanic.Domain.Shared.References.Identity;

public interface IIdentityReference
{
    Guid IdentityId { get; }
    string Email { get; }
}

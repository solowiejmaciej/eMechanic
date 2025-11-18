namespace eMechanic.Domain.References.Identity;

public interface IIdentityReference
{
    Guid IdentityId { get; }
    string Email { get; }
}

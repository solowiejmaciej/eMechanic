namespace eMechanic.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

public sealed class Identity : IdentityUser<Guid>
{
    public EIdentityType Type { get; private set; }

    private Identity() { }

    private Identity(string email, EIdentityType type)
    {
        UserName = email;
        Email = email;
        Type = type;
    }

    public static Identity Create(string email, EIdentityType type) => new(email, type);
}

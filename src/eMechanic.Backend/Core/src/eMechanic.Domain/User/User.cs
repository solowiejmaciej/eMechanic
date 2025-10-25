namespace eMechanic.Domain.User;

using DomainEvents;
using eMechanic.Common.DDD;

public class User : AggregateRoot
{
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Guid IdentityId { get; private set; }

    private User()
    {
    }

    private User(string email, string firstName, string lastName, Guid identityId)
    {
        SetEmail(email);
        SetFirstName(firstName);
        SetLastName(lastName);
        SetIdentityId(identityId);
    }

    public static User Create(string email, string firstName, string lastName, Guid identityId)
    {
        var user = new User(email, firstName, lastName, identityId);

        user.RaiseDomainEvent(new UserCreatedDomainEvent(user));

        return user;
    }

    private void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty.", nameof(email));
        }

        Email = email;
    }

    private void SetIdentityId(Guid identityId)
    {
        if (identityId == Guid.Empty)
        {
            throw new ArgumentException("Identity ID cannot be empty.", nameof(identityId));
        }

        IdentityId = identityId;
    }

    private void SetFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name cannot be empty.", nameof(firstName));
        }

        FirstName = firstName;
    }

    private void SetLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name cannot be empty.", nameof(lastName));
        }

        LastName = lastName;
    }
}

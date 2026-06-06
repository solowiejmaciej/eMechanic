namespace eMechanic.Domain.User;

using DomainEvents;
using eMechanic.Common.DDD;
using Shared.References.Identity;
using Shared.ValueObjects;

public class User : AggregateRoot, IIdentityReference
{
    public Email Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Guid IdentityId { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }

    private User()
    {
    }

    private User(Email email, string firstName, string lastName, Guid identityId, PhoneNumber? phoneNumber)
    {
        SetEmail(email);
        SetFirstName(firstName);
        SetLastName(lastName);
        SetIdentityId(identityId);
        PhoneNumber = phoneNumber;
    }

    public static User Create(string email, string firstName, string lastName, Guid identityId, string? phoneNumber = null)
    {
        var emailResult = Email.Create(email);
        if (emailResult.HasError())
            throw new ArgumentException(emailResult.Error!.Message, nameof(email));

        PhoneNumber? phoneNumberVo = null;
        if (!string.IsNullOrWhiteSpace(phoneNumber))
        {
            var phoneResult = PhoneNumber.Create(phoneNumber);
            if (phoneResult.HasError())
                throw new ArgumentException(phoneResult.Error!.Message, nameof(phoneNumber));
            phoneNumberVo = phoneResult.Value;
        }

        var user = new User(emailResult.Value!, firstName, lastName, identityId, phoneNumberVo);
        user.RaiseDomainEvent(new UserCreatedDomainEvent(user));
        return user;
    }

    public void Update(string email, string firstName, string lastName, string? phoneNumber = null)
    {
        var emailResult = Email.Create(email);
        if (emailResult.HasError())
            throw new ArgumentException(emailResult.Error!.Message, nameof(email));

        SetEmail(emailResult.Value!);
        SetFirstName(firstName);
        SetLastName(lastName);

        PhoneNumber? phoneNumberVo = null;
        if (!string.IsNullOrWhiteSpace(phoneNumber))
        {
            var phoneResult = PhoneNumber.Create(phoneNumber);
            if (phoneResult.HasError())
                throw new ArgumentException(phoneResult.Error!.Message, nameof(phoneNumber));
            phoneNumberVo = phoneResult.Value;
        }
        PhoneNumber = phoneNumberVo;

        RaiseDomainEvent(new UserUpdatedDomainEvent(this));
    }

    private void SetEmail(Email email)
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
    }

    private void SetIdentityId(Guid identityId)
    {
        if (identityId == Guid.Empty)
            throw new ArgumentException("Identity ID cannot be empty.", nameof(identityId));
        IdentityId = identityId;
    }

    private void SetFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty.", nameof(firstName));
        FirstName = firstName;
    }

    private void SetLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty.", nameof(lastName));
        LastName = lastName;
    }
}

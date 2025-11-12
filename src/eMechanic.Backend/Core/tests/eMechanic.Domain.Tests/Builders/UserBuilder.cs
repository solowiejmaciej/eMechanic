namespace eMechanic.Domain.Tests.Builders;

using eMechanic.Domain.User;

public class UserBuilder
{
    private string _email = "test@user.pl";
    private string _firstName = "Test";
    private string _lastName = "User";
    private Guid _identityId = Guid.NewGuid();

    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UserBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public UserBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public UserBuilder WithIdentityId(Guid identityId)
    {
        _identityId = identityId;
        return this;
    }

    public User Build()
    {
        return User.Create(_email, _firstName, _lastName, _identityId);
    }
}

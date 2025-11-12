namespace eMechanic.Application.Tests.Builders;

using eMechanic.Application.Users.Features.Update;

public class UpdateUserCommandBuilder
{
    private string _firstName = "John";
    private string _lastName = "Doe";
    private string _email = "john.doe@example.com";

    public UpdateUserCommandBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public UpdateUserCommandBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public UpdateUserCommandBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UpdateUserCommand Build()
    {
        return new UpdateUserCommand(_firstName, _lastName, _email);
    }
}

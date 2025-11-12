namespace eMechanic.Application.Tests.Builders;

using eMechanic.Application.Users.Features.Create;

public class CreateUserCommandBuilder
{
    private string _firstName = "John";
    private string _lastName = "Doe";
    private string _email = "john.doe@example.com";
    private string _password = "Password123!";

    public CreateUserCommandBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public CreateUserCommandBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public CreateUserCommandBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public CreateUserCommandBuilder WithPassword(string password)
    {
        _password = password;
        return this;
    }

    public CreateUserCommand Build()
    {
        return new CreateUserCommand(_firstName, _lastName, _email, _password);
    }
}

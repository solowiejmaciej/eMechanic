namespace eMechanic.Application.Tests.Builders;

using eMechanic.Application.Token.Features.Create.User;

public class CreateUserTokenCommandBuilder
{
    private string _email = "test@user.com";
    private string _password = "Password123";

    public CreateUserTokenCommandBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public CreateUserTokenCommandBuilder WithPassword(string password)
    {
        _password = password;
        return this;
    }

    public CreateUserTokenCommand Build()
    {
        return new CreateUserTokenCommand(_email, _password);
    }
}

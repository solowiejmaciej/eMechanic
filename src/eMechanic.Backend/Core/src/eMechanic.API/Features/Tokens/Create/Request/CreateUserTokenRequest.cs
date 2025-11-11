namespace eMechanic.API.Features.Tokens.Create.Request;

using Application.Token.Features.Create.User;

public sealed record CreateUserTokenRequest(
    string Email,
    string Password)
{
    public CreateUserTokenCommand MapToCommand() => new(Email, Password);
}


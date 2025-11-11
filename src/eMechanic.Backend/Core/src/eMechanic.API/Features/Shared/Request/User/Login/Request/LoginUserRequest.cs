namespace eMechanic.API.Features.User.Login.Request;

using eMechanic.Application.Users.Features.Login;

public sealed record LoginUserRequest(
    string Email,
    string Password)
{
    public LoginUserCommand MapToCommand() => new(Email, Password);
}


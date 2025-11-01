namespace eMechanic.API.Features.User.Login;

using eMechanic.Application.Users.Login;

public sealed record LoginUserRequest(
    string Email,
    string Password)
{
    public LoginUserCommand MapToCommand() => new(Email, Password);
}


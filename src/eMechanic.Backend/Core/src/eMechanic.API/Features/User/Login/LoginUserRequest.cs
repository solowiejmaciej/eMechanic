namespace eMechanic.API.Features.User.Login;

using Application.Users.Features.Login;

public sealed record LoginUserRequest(
    string Email,
    string Password)
{
    public LoginUserCommand MapToCommand() => new(Email, Password);
}


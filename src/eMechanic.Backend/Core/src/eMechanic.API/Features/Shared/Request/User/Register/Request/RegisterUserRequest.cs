namespace eMechanic.API.Features.User.Register.Request;

using eMechanic.Application.Users.Features.Register;

public sealed record RegisterUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password)
{
    public RegisterUserCommand MapToCommand() => new(FirstName, LastName, Email, Password);
}

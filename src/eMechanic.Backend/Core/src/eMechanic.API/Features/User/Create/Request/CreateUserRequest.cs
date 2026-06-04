namespace eMechanic.API.Features.User.Create.Request;

using Application.Users.Features.Create;

public sealed record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string? PhoneNumber = null)
{
    public CreateUserCommand MapToCommand() => new(FirstName, LastName, Email, Password, PhoneNumber);
}

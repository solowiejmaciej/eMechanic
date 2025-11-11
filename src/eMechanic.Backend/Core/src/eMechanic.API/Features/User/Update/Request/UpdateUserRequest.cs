namespace eMechanic.API.Features.User.Update.Request;

using Application.Users.Features.Update;

public sealed record UpdateUserRequest(
    string FirstName,
    string LastName,
    string Email)
{
    public UpdateUserCommand MapToCommand() => new(FirstName, LastName, Email);
}

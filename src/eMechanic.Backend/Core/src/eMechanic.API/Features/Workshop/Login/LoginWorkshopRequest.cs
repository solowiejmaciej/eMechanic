namespace eMechanic.API.Features.Workshop.Login;

using eMechanic.Application.Workshop.Login;

public sealed record LoginWorkshopRequest(string Email, string Password)
{
    public LoginWorkshopCommand MapToCommand() => new(Email, Password);
}


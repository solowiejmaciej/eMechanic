namespace eMechanic.API.Features.Workshop.Login;

using Application.Workshop.Features.Login;

public sealed record LoginWorkshopRequest(string Email, string Password)
{
    public LoginWorkshopCommand MapToCommand() => new(Email, Password);
}


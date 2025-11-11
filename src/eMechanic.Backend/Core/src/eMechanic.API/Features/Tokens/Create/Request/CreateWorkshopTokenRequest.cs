namespace eMechanic.API.Features.Tokens.Create.Request;

using Application.Token.Features.Create.Workshop;

public sealed record CreateWorkshopTokenRequest(string Email, string Password)
{
    public CreateWorkshopTokenCommand MapToCommand() => new(Email, Password);
}


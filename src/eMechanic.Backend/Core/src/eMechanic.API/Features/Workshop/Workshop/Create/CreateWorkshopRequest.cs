namespace eMechanic.API.Features.Workshop.Workshop.Create;

using eMechanic.Application.Workshop.Workshop.Features.Create;

public sealed record CreateWorkshopRequest(
    string Email,
    string Password,
    string ContactEmail,
    string Name,
    string DisplayName,
    string PhoneNumber,
    string Address,
    string City,
    string PostalCode,
    string Country)
{
    public CreateWorkshopCommand MapToCommand() => new(
        Email,
        Password,
        ContactEmail,
        Name,
        DisplayName,
        PhoneNumber,
        Address,
        City,
        PostalCode,
        Country);
}

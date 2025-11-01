namespace eMechanic.API.Features.Workshop.Register;

using Application.Workshop.Register;

public sealed record RegisterWorkshopRequest(
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
    public RegisterWorkshopCommand MapToCommand() => new(
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

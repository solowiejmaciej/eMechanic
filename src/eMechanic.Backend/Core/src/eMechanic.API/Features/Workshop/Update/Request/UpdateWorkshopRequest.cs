namespace eMechanic.API.Features.Workshop.Update.Request;

using eMechanic.Application.Workshop.Features.Update;

public sealed record UpdateWorkshopRequest(
    string Email,
    string ContactEmail,
    string Name,
    string DisplayName,
    string PhoneNumber,
    string Address,
    string City,
    string PostalCode,
    string Country)
{
    public UpdateWorkshopCommand MapToCommand() => new(
        Email,
        ContactEmail,
        Name,
        DisplayName,
        PhoneNumber,
        Address,
        City,
        PostalCode,
        Country);
}

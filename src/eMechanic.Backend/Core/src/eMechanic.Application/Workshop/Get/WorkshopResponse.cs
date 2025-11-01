namespace eMechanic.Application.Workshop.Get;

public sealed record WorkshopResponse(Guid Id, string ContactEmail, string DisplayName, string PhoneNumber, string Address, string City, string PostalCode, string Country);

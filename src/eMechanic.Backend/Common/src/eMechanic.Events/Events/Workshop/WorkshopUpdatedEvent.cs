namespace eMechanic.Events.Events.Workshop;

public class WorkshopUpdatedEvent : EventBase
{
    public Guid WorkshopId { get; private set; }
    public string Email { get; private set; }
    public string ContactEmail { get; private set; }
    public string Name { get; private set; }
    public string DisplayName { get; private set; }
    public string PhoneNumber { get; private set; }
    public string Address { get; private set; }
    public string City { get; private set; }
    public string PostalCode { get; private set; }
    public string Country { get; private set; }

    public WorkshopUpdatedEvent(Guid workshopId, string email, string contactEmail, string name, string displayName, string phoneNumber, string address, string city, string postalCode, string country)
    {
        WorkshopId = workshopId;
        Email = email;
        ContactEmail = contactEmail;
        Name = name;
        DisplayName = displayName;
        PhoneNumber = phoneNumber;
        Address = address;
        City = city;
        PostalCode = postalCode;
        Country = country;
    }
}

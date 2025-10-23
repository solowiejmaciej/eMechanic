namespace eMechanic.Domain.Workshop;

using Common.DDD;
using DomainEvents;

public class Workshop : AggregateRoot
{
    public string Email { get; private set; }
    public string ContactEmail { get; private set; }
    public string Name { get; private set; }
    public string DisplayName { get; private set; }
    public string PhoneNumber { get; private set; }
    public string Address { get; private set; }
    public string City { get; private set; }
    public string PostalCode { get; private set; }
    public string Country { get; private set; }
    public Guid IdentityId { get; private set; }

    public static Workshop Create(
        string contactEmail,
        string email,
        string displayName,
        string name,
        string phoneNumber,
        string address,
        string city,
        string postalCode,
        string country,
        Guid identityId)
    {
        return new Workshop(
            contactEmail,
            email,
            displayName,
            name,
            phoneNumber,
            address,
            city,
            postalCode,
            country,
            identityId);
    }

    private Workshop()
    {
    }

    private Workshop(
        string contactEmail,
        string email,
        string displayName,
        string name,
        string phoneNumber,
        string address,
        string city,
        string postalCode,
        string country,
        Guid identityId)
    {
        SetEmail(email);
        SetContactEmail(contactEmail);
        SetName(name);
        SetDisplayName(displayName);
        SetPhoneNumber(phoneNumber);
        SetAddress(address);
        SetCity(city);
        SetPostalCode(postalCode);
        SetCountry(country);
        SetIdentityId(identityId);

        RaiseDomainEvent(new WorkshopCreatedDomainEvent(this));
    }

    private void SetIdentityId(Guid identityId)
    {
        if (identityId == Guid.Empty)
        {
            throw new ArgumentException("Identity ID cannot be empty.", nameof(identityId));
        }

        IdentityId = identityId;
    }

    private void SetContactEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Contact email cannot be empty.", nameof(email));
        }

        ContactEmail = email;
    }

    private void SetDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Display name cannot be empty.", nameof(displayName));
        }

        DisplayName = displayName;
    }

    private void SetPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new ArgumentException("Phone number cannot be empty.", nameof(phoneNumber));
        }

        PhoneNumber = phoneNumber;
    }

    private void SetAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("Address cannot be empty.", nameof(address));
        }

        Address = address;
    }

    private void SetCity(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("City cannot be empty.", nameof(city));
        }

        City = city;
    }

    private void SetPostalCode(string postalCode)
    {
        if (string.IsNullOrWhiteSpace(postalCode))
        {
            throw new ArgumentException("Postal code cannot be empty.", nameof(postalCode));
        }

        PostalCode = postalCode;
    }

    private void SetCountry(string country)
    {
        if (string.IsNullOrWhiteSpace(country))
        {
            throw new ArgumentException("Country cannot be empty.", nameof(country));
        }

        Country = country;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        }

        Name = name;
    }

    private void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty.", nameof(email));
        }

        Email = email;
    }
}

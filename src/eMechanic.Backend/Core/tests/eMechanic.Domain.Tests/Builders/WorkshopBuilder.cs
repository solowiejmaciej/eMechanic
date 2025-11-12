namespace eMechanic.Domain.Tests.Builders;

using eMechanic.Domain.Workshop;

public class WorkshopBuilder
{
    private string _contactEmail = "kontakt@warsztat.pl";
    private string _email = "login@warsztat.pl";
    private string _displayName = "Janex";
    private string _name = "Auto-Serwis Jan";
    private string _phoneNumber = "123456789";
    private string _address = "Adres";
    private string _city = "Miasto";
    private string _postalCode = "Kod";
    private string _country = "Kraj";
    private Guid _identityId = Guid.NewGuid();

    public WorkshopBuilder WithContactEmail(string contactEmail)
    {
        _contactEmail = contactEmail;
        return this;
    }

    public WorkshopBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public WorkshopBuilder WithDisplayName(string displayName)
    {
        _displayName = displayName;
        return this;
    }

    public WorkshopBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public WorkshopBuilder WithPhoneNumber(string phoneNumber)
    {
        _phoneNumber = phoneNumber;
        return this;
    }

    public WorkshopBuilder WithAddress(string address)
    {
        _address = address;
        return this;
    }

    public WorkshopBuilder WithCity(string city)
    {
        _city = city;
        return this;
    }

    public WorkshopBuilder WithPostalCode(string postalCode)
    {
        _postalCode = postalCode;
        return this;
    }

    public WorkshopBuilder WithCountry(string country)
    {
        _country = country;
        return this;
    }

    public WorkshopBuilder WithIdentityId(Guid identityId)
    {
        _identityId = identityId;
        return this;
    }

    public Workshop Build()
    {
        return Workshop.Create(_contactEmail, _email, _displayName, _name, _phoneNumber, _address, _city, _postalCode, _country, _identityId);
    }
}

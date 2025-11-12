namespace eMechanic.Application.Tests.Builders;

using eMechanic.Application.Workshop.Features.Create;

public class CreateWorkshopCommandBuilder
{
    private string _email = "login@warsztat.pl";
    private string _password = "ValidPassword123";
    private string _contactEmail = "kontakt@warsztat.pl";
    private string _name = "Auto-Serwis Jan";
    private string _displayName = "Janex";
    private string _phoneNumber = "123456789";
    private string _address = "ul. Warsztatowa 1";
    private string _city = "Warszawa";
    private string _postalCode = "00-001";
    private string _country = "Polska";

    public CreateWorkshopCommandBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public CreateWorkshopCommandBuilder WithPassword(string password)
    {
        _password = password;
        return this;
    }

    public CreateWorkshopCommandBuilder WithContactEmail(string contactEmail)
    {
        _contactEmail = contactEmail;
        return this;
    }

    public CreateWorkshopCommandBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public CreateWorkshopCommandBuilder WithDisplayName(string displayName)
    {
        _displayName = displayName;
        return this;
    }

    public CreateWorkshopCommandBuilder WithPhoneNumber(string phoneNumber)
    {
        _phoneNumber = phoneNumber;
        return this;
    }

    public CreateWorkshopCommandBuilder WithAddress(string address)
    {
        _address = address;
        return this;
    }

    public CreateWorkshopCommandBuilder WithCity(string city)
    {
        _city = city;
        return this;
    }

    public CreateWorkshopCommandBuilder WithPostalCode(string postalCode)
    {
        _postalCode = postalCode;
        return this;
    }

    public CreateWorkshopCommandBuilder WithCountry(string country)
    {
        _country = country;
        return this;
    }

    public CreateWorkshopCommand Build()
    {
        return new CreateWorkshopCommand(
            _email,
            _password,
            _contactEmail,
            _name,
            _displayName,
            _phoneNumber,
            _address,
            _city,
            _postalCode,
            _country);
    }
}

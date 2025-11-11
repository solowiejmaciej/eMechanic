using FluentValidation.TestHelper;

namespace eMechanic.Application.Tests.Workshop.Features.Update;

using Application.Workshop.Features.Update;

public class UpdateWorkshopCommandValidatorTests
{
    private readonly UpdateWorkshopCommandValidator _validator;

    public UpdateWorkshopCommandValidatorTests()
    {
        _validator = new UpdateWorkshopCommandValidator();
    }

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValid()
    {
        var command = new UpdateWorkshopCommand(
            "login@warsztat.pl", "kontakt@warsztat.pl",
            "Auto-Serwis Jan", "Janex", "123456789",
            "ul. Warsztatowa 1", "Warszawa", "00-001", "Polska");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("nie-email")]
    public void Should_HaveError_WhenEmailIsInvalid(string email)
    {
        var command = new UpdateWorkshopCommand(
            email, "kontakt@warsztat.pl", "Nazwa", "Display",
            "123", "Adres", "Miasto", "Kod", "Kraj");

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("nie-email")]
    public void Should_HaveError_WhenContactEmailIsInvalid(string contactEmail)
    {
        var command = new UpdateWorkshopCommand(
            "login@warsztat.pl", contactEmail, "Nazwa", "Display",
            "123", "Adres", "Miasto", "Kod", "Kraj");

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.ContactEmail);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_HaveError_WhenNameIsEmpty(string name)
    {
        var command = new UpdateWorkshopCommand(
            "login@warsztat.pl", "kontakt@warsztat.pl", name, "Display",
            "123", "Adres", "Miasto", "Kod", "Kraj");

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_HaveError_WhenDisplayNameIsEmpty(string displayName)
    {
        var command = new UpdateWorkshopCommand(
            "login@warsztat.pl", "kontakt@warsztat.pl", "Nazwa", displayName,
            "123", "Adres", "Miasto", "Kod", "Kraj");

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.DisplayName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_HaveError_WhenPhoneNumberIsEmpty(string phone)
    {
        var command = new UpdateWorkshopCommand(
            "login@warsztat.pl", "kontakt@warsztat.pl", "Nazwa", "Display",
            phone, "Adres", "Miasto", "Kod", "Kraj");

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.PhoneNumber);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_HaveError_WhenAddressIsEmpty(string address)
    {
        var command = new UpdateWorkshopCommand(
            "login@warsztat.pl", "kontakt@warsztat.pl", "Nazwa", "Display",
            "123", address, "Miasto", "Kod", "Kraj");

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Address);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_HaveError_WhenCityIsEmpty(string city)
    {
        var command = new UpdateWorkshopCommand(
            "login@warsztat.pl", "kontakt@warsztat.pl", "Nazwa", "Display",
            "123", "Adres", city, "Kod", "Kraj");

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.City);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_HaveError_WhenPostalCodeIsEmpty(string postalCode)
    {
        var command = new UpdateWorkshopCommand(
            "login@warsztat.pl", "kontakt@warsztat.pl", "Nazwa", "Display",
            "123", "Adres", "Miasto", postalCode, "Kraj");

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.PostalCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_HaveError_WhenCountryIsEmpty(string country)
    {
        var command = new UpdateWorkshopCommand(
            "login@warsztat.pl", "kontakt@warsztat.pl", "Nazwa", "Display",
            "123", "Adres", "Miasto", "Kod", country);

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Country);
    }
}

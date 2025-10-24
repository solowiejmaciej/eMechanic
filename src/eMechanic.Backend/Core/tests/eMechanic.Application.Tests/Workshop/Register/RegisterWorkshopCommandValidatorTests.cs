using eMechanic.Application.Workshop.Register;
using FluentValidation.TestHelper;

namespace eMechanic.Application.Tests.Workshop.Register
{
    public class RegisterWorkshopCommandValidatorTests
    {
        private readonly RegisterWorkshopCommandValidator _validator;

        public RegisterWorkshopCommandValidatorTests()
        {
            _validator = new RegisterWorkshopCommandValidator();
        }

        [Fact]
        public void Should_NotHaveError_WhenCommandIsValid()
        {
            var command = new RegisterWorkshopCommand(
                "login@warsztat.pl", "ValidPassword123", "kontakt@warsztat.pl",
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
            var command = new RegisterWorkshopCommand(
                email, "ValidPassword123", "kontakt@warsztat.pl", "Nazwa", "Display",
                "123", "Adres", "Miasto", "Kod", "Kraj");

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Email);
        }

        [Theory]
        [InlineData("pass")]
        [InlineData("passwordbezcyfry")]
        public void Should_HaveError_WhenPasswordIsInvalid(string password)
        {
            var command = new RegisterWorkshopCommand(
                "login@warsztat.pl", password, "kontakt@warsztat.pl", "Nazwa", "Display",
                "123", "Adres", "Miasto", "Kod", "Kraj");

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Password);
        }

        [Fact]
        public void Should_HaveError_WhenNameIsEmpty()
        {
            var command = new RegisterWorkshopCommand(
                "login@warsztat.pl", "ValidPassword123", "kontakt@warsztat.pl", "", "Display",
                "123", "Adres", "Miasto", "Kod", "Kraj");

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Name)
                  .WithErrorMessage("Name cannot be empty.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("nie-email")]
        public void Should_HaveError_WhenContactEmailIsInvalid(string contactEmail)
        {
            var command = new RegisterWorkshopCommand(
                "login@warsztat.pl", "ValidPassword123", contactEmail, "Nazwa", "Display",
                "123", "Adres", "Miasto", "Kod", "Kraj");

            var result = _validator.TestValidate(command);

            // empty and whitespace are covered; invalid format triggers email validation
            if (string.IsNullOrWhiteSpace(contactEmail))
            {
                result.ShouldHaveValidationErrorFor(c => c.ContactEmail)
                      .WithErrorMessage("Contact email cannot be empty.");
            }
            else
            {
                result.ShouldHaveValidationErrorFor(c => c.ContactEmail)
                      .WithErrorMessage("A valid contact email address is required.");
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_HaveError_WhenDisplayNameIsEmpty(string displayName)
        {
            var command = new RegisterWorkshopCommand(
                "login@warsztat.pl", "ValidPassword123", "kontakt@warsztat.pl", "Nazwa", displayName,
                "123", "Adres", "Miasto", "Kod", "Kraj");

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.DisplayName)
                  .WithErrorMessage("Display name cannot be empty.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_HaveError_WhenPhoneNumberIsEmpty(string phone)
        {
            var command = new RegisterWorkshopCommand(
                "login@warsztat.pl", "ValidPassword123", "kontakt@warsztat.pl", "Nazwa", "Display",
                phone, "Adres", "Miasto", "Kod", "Kraj");

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.PhoneNumber)
                  .WithErrorMessage("Phone number cannot be empty.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_HaveError_WhenAddressIsEmpty(string address)
        {
            var command = new RegisterWorkshopCommand(
                "login@warsztat.pl", "ValidPassword123", "kontakt@warsztat.pl", "Nazwa", "Display",
                "123", address, "Miasto", "Kod", "Kraj");

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Address)
                  .WithErrorMessage("Address cannot be empty.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_HaveError_WhenCityIsEmpty(string city)
        {
            var command = new RegisterWorkshopCommand(
                "login@warsztat.pl", "ValidPassword123", "kontakt@warsztat.pl", "Nazwa", "Display",
                "123", "Adres", city, "Kod", "Kraj");

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.City)
                  .WithErrorMessage("City cannot be empty.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_HaveError_WhenPostalCodeIsEmpty(string postal)
        {
            var command = new RegisterWorkshopCommand(
                "login@warsztat.pl", "ValidPassword123", "kontakt@warsztat.pl", "Nazwa", "Display",
                "123", "Adres", "Miasto", postal, "Kraj");

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.PostalCode)
                  .WithErrorMessage("Postal code cannot be empty.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_HaveError_WhenCountryIsEmpty(string country)
        {
            var command = new RegisterWorkshopCommand(
                "login@warsztat.pl", "ValidPassword123", "kontakt@warsztat.pl", "Nazwa", "Display",
                "123", "Adres", "Miasto", "Kod", country);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Country)
                  .WithErrorMessage("Country cannot be empty.");
        }
    }
}

namespace eMechanic.Application.Tests.Workshop.Features.Create
{
    using Application.Tests.Builders;
    using Application.Workshop.Features.Create;
    using FluentValidation.TestHelper;

    public class CreateWorkshopCommandValidatorTests
    {
        private readonly CreateWorkshopCommandValidator _validator;

        public CreateWorkshopCommandValidatorTests()
        {
            _validator = new CreateWorkshopCommandValidator();
        }

        [Fact]
        public void Should_NotHaveError_WhenCommandIsValid()
        {
            var command = new CreateWorkshopCommandBuilder().Build();

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("nie-email")]
        public void Should_HaveError_WhenEmailIsInvalid(string email)
        {
            var command = new CreateWorkshopCommandBuilder().WithEmail(email).Build();

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Email);
        }

        [Theory]
        [InlineData("pass")]
        [InlineData("passwordbezcyfry")]
        public void Should_HaveError_WhenPasswordIsInvalid(string password)
        {
            var command = new CreateWorkshopCommandBuilder().WithPassword(password).Build();

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Password);
        }

        [Fact]
        public void Should_HaveError_WhenNameIsEmpty()
        {
            var command = new CreateWorkshopCommandBuilder().WithName("").Build();

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
            var command = new CreateWorkshopCommandBuilder().WithContactEmail(contactEmail).Build();

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
            var command = new CreateWorkshopCommandBuilder().WithDisplayName(displayName).Build();

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.DisplayName)
                  .WithErrorMessage("Display name cannot be empty.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_HaveError_WhenPhoneNumberIsEmpty(string phone)
        {
            var command = new CreateWorkshopCommandBuilder().WithPhoneNumber(phone).Build();

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.PhoneNumber)
                  .WithErrorMessage("Phone number cannot be empty.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_HaveError_WhenAddressIsEmpty(string address)
        {
            var command = new CreateWorkshopCommandBuilder().WithAddress(address).Build();

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Address)
                  .WithErrorMessage("Address cannot be empty.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_HaveError_WhenCityIsEmpty(string city)
        {
            var command = new CreateWorkshopCommandBuilder().WithCity(city).Build();

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.City)
                  .WithErrorMessage("City cannot be empty.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_HaveError_WhenPostalCodeIsEmpty(string postal)
        {
            var command = new CreateWorkshopCommandBuilder().WithPostalCode(postal).Build();

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.PostalCode)
                  .WithErrorMessage("Postal code cannot be empty.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_HaveError_WhenCountryIsEmpty(string country)
        {
            var command = new CreateWorkshopCommandBuilder().WithCountry(country).Build();

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Country)
                  .WithErrorMessage("Country cannot be empty.");
        }
    }
}

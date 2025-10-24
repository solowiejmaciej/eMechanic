using System.Collections;
using eMechanic.Domain.Workshop.DomainEvents;

namespace eMechanic.Domain.Tests.Workshop
{
    public class WorkshopTests
    {
        [Fact]
        public void Create_Should_SuccessfullyCreateWorkshop_WhenDataIsValid()
        {
            // Arrange
            var contactEmail = "kontakt@warsztat.pl";
            var email = "login@warsztat.pl";
            var displayName = "Janex";
            var name = "Auto-Serwis Jan";
            var phoneNumber = "123456789";
            var identityId = Guid.NewGuid();

            // Act
            var workshop = Domain.Workshop.Workshop.Create(
                contactEmail, email, displayName, name, phoneNumber,
                "Adres", "Miasto", "Kod", "Kraj", identityId);

            // Assert
            Assert.NotNull(workshop);
            Assert.Equal(email, workshop.Email);
            Assert.Equal(name, workshop.Name);
            Assert.Equal(displayName, workshop.DisplayName);
            Assert.Equal(identityId, workshop.IdentityId);
            Assert.NotEqual(Guid.Empty, workshop.Id);
        }

        [Fact]
        public void Create_Should_RaiseWorkshopCreatedDomainEvent_WhenCreated()
        {
            // Act
            var workshop = Domain.Workshop.Workshop.Create(
                "kontakt@warsztat.pl", "login@warsztat.pl", "Janex", "Auto-Serwis Jan", "123456789",
                "Adres", "Miasto", "Kod", "Kraj", Guid.NewGuid());

            var domainEvents = workshop.GetDomainEvents();

            // Assert
            Assert.NotNull(domainEvents);
            Assert.Single((IEnumerable)domainEvents);
            Assert.IsType<WorkshopCreatedDomainEvent>(domainEvents.First());
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_Should_ThrowArgumentException_WhenNameIsEmpty(string invalidName)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                Domain.Workshop.Workshop.Create(
                    "kontakt@warsztat.pl", "login@warsztat.pl", "Janex", invalidName, "123456789",
                    "Adres", "Miasto", "Kod", "Kraj", Guid.NewGuid())
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_Should_ThrowArgumentException_WhenDisplayNameIsEmpty(string invalidDisplayName)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                Domain.Workshop.Workshop.Create(
                    "kontakt@warsztat.pl", "login@warsztat.pl", invalidDisplayName, "Nazwa", "123456789",
                    "Adres", "Miasto", "Kod", "Kraj", Guid.NewGuid())
            );
        }

        [Fact]
        public void Create_Should_ThrowArgumentException_WhenIdentityIdIsEmpty()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                Domain.Workshop.Workshop.Create(
                    "kontakt@warsztat.pl", "login@warsztat.pl", "Janex", "Nazwa", "123456789",
                    "Adres", "Miasto", "Kod", "Kraj", Guid.Empty)
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_Should_ThrowArgumentException_WhenEmailIsEmpty(string invalidEmail)
        {
            Assert.Throws<ArgumentException>(() =>
                Domain.Workshop.Workshop.Create(
                    "kontakt@warsztat.pl", invalidEmail, "Janex", "Nazwa", "123456789",
                    "Adres", "Miasto", "Kod", "Kraj", Guid.NewGuid())
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_Should_ThrowArgumentException_WhenContactEmailIsEmpty(string invalidContactEmail)
        {
            Assert.Throws<ArgumentException>(() =>
                Domain.Workshop.Workshop.Create(
                    invalidContactEmail, "login@warsztat.pl", "Janex", "Nazwa", "123456789",
                    "Adres", "Miasto", "Kod", "Kraj", Guid.NewGuid())
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_Should_ThrowArgumentException_WhenPhoneNumberIsEmpty(string invalidPhone)
        {
            Assert.Throws<ArgumentException>(() =>
                Domain.Workshop.Workshop.Create(
                    "kontakt@warsztat.pl", "login@warsztat.pl", "Janex", "Nazwa", invalidPhone,
                    "Adres", "Miasto", "Kod", "Kraj", Guid.NewGuid())
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_Should_ThrowArgumentException_WhenAddressIsEmpty(string invalidAddress)
        {
            Assert.Throws<ArgumentException>(() =>
                Domain.Workshop.Workshop.Create(
                    "kontakt@warsztat.pl", "login@warsztat.pl", "Janex", "Nazwa", "123456789",
                    invalidAddress, "Miasto", "Kod", "Kraj", Guid.NewGuid())
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_Should_ThrowArgumentException_WhenCityIsEmpty(string invalidCity)
        {
            Assert.Throws<ArgumentException>(() =>
                Domain.Workshop.Workshop.Create(
                    "kontakt@warsztat.pl", "login@warsztat.pl", "Janex", "Nazwa", "123456789",
                    "Adres", invalidCity, "Kod", "Kraj", Guid.NewGuid())
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_Should_ThrowArgumentException_WhenPostalCodeIsEmpty(string invalidPostal)
        {
            Assert.Throws<ArgumentException>(() =>
                Domain.Workshop.Workshop.Create(
                    "kontakt@warsztat.pl", "login@warsztat.pl", "Janex", "Nazwa", "123456789",
                    "Adres", "Miasto", invalidPostal, "Kraj", Guid.NewGuid())
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_Should_ThrowArgumentException_WhenCountryIsEmpty(string invalidCountry)
        {
            Assert.Throws<ArgumentException>(() =>
                Domain.Workshop.Workshop.Create(
                    "kontakt@warsztat.pl", "login@warsztat.pl", "Janex", "Nazwa", "123456789",
                    "Adres", "Miasto", "Kod", invalidCountry, Guid.NewGuid())
            );
        }

        [Fact]
        public void Create_Should_RaiseWorkshopCreatedDomainEvent_WithCorrectWorkshopReference()
        {
            var workshop = Domain.Workshop.Workshop.Create(
                "kontakt@warsztat.pl", "login@warsztat.pl", "Janex", "Auto-Serwis Jan", "123456789",
                "Adres", "Miasto", "Kod", "Kraj", Guid.NewGuid());

            var domainEvent = workshop.GetDomainEvents().First() as WorkshopCreatedDomainEvent;

            Assert.NotNull(domainEvent);
            Assert.Equal(workshop.Id, domainEvent.Workshop.Id);
        }

        [Fact]
        public void Create_Should_GenerateUniqueIds_ForMultipleWorkshops()
        {
            var w1 = Domain.Workshop.Workshop.Create(
                "kontakt1@warsztat.pl", "login1@warsztat.pl", "Disp1", "Nazwa1", "111111111",
                "Adres1", "Miasto1", "Kod1", "Kraj1", Guid.NewGuid());

            var w2 = Domain.Workshop.Workshop.Create(
                "kontakt2@warsztat.pl", "login2@warsztat.pl", "Disp2", "Nazwa2", "222222222",
                "Adres2", "Miasto2", "Kod2", "Kraj2", Guid.NewGuid());

            Assert.NotEqual(w1.Id, w2.Id);
            Assert.NotEqual(Guid.Empty, w1.Id);
            Assert.NotEqual(Guid.Empty, w2.Id);
        }
    }
}

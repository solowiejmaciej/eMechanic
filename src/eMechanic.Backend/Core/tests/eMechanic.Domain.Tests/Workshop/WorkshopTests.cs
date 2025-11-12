using System.Collections;
using eMechanic.Domain.Tests.Builders;
using eMechanic.Domain.Workshop.DomainEvents;

namespace eMechanic.Domain.Tests.Workshop;

using FluentAssertions;

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
        var workshop = new WorkshopBuilder()
            .WithContactEmail(contactEmail)
            .WithEmail(email)
            .WithDisplayName(displayName)
            .WithName(name)
            .WithPhoneNumber(phoneNumber)
            .WithIdentityId(identityId)
            .Build();

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
        var workshop = new WorkshopBuilder().Build();

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
            new WorkshopBuilder().WithName(invalidName).Build()
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ThrowArgumentException_WhenDisplayNameIsEmpty(string invalidDisplayName)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new WorkshopBuilder().WithDisplayName(invalidDisplayName).Build()
        );
    }

    [Fact]
    public void Create_Should_ThrowArgumentException_WhenIdentityIdIsEmpty()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new WorkshopBuilder().WithIdentityId(Guid.Empty).Build()
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ThrowArgumentException_WhenEmailIsEmpty(string invalidEmail)
    {
        Assert.Throws<ArgumentException>(() =>
            new WorkshopBuilder().WithEmail(invalidEmail).Build()
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ThrowArgumentException_WhenContactEmailIsEmpty(string invalidContactEmail)
    {
        Assert.Throws<ArgumentException>(() =>
            new WorkshopBuilder().WithContactEmail(invalidContactEmail).Build()
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ThrowArgumentException_WhenPhoneNumberIsEmpty(string invalidPhone)
    {
        Assert.Throws<ArgumentException>(() =>
            new WorkshopBuilder().WithPhoneNumber(invalidPhone).Build()
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ThrowArgumentException_WhenAddressIsEmpty(string invalidAddress)
    {
        Assert.Throws<ArgumentException>(() =>
            new WorkshopBuilder().WithAddress(invalidAddress).Build()
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ThrowArgumentException_WhenCityIsEmpty(string invalidCity)
    {
        Assert.Throws<ArgumentException>(() =>
            new WorkshopBuilder().WithCity(invalidCity).Build()
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ThrowArgumentException_WhenPostalCodeIsEmpty(string invalidPostal)
    {
        Assert.Throws<ArgumentException>(() =>
            new WorkshopBuilder().WithPostalCode(invalidPostal).Build()
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ThrowArgumentException_WhenCountryIsEmpty(string invalidCountry)
    {
        Assert.Throws<ArgumentException>(() =>
            new WorkshopBuilder().WithCountry(invalidCountry).Build()
        );
    }

    [Fact]
    public void Create_Should_RaiseWorkshopCreatedDomainEvent_WithCorrectWorkshopReference()
    {
        var workshop = new WorkshopBuilder().Build();

        var domainEvent = workshop.GetDomainEvents().First() as WorkshopCreatedDomainEvent;

        Assert.NotNull(domainEvent);
        Assert.Equal(workshop.Id, domainEvent.Workshop.Id);
    }

    [Fact]
    public void Create_Should_GenerateUniqueIds_ForMultipleWorkshops()
    {
        var w1 = new WorkshopBuilder().Build();
        var w2 = new WorkshopBuilder().Build();

        Assert.NotEqual(w1.Id, w2.Id);
        Assert.NotEqual(Guid.Empty, w1.Id);
        Assert.NotEqual(Guid.Empty, w2.Id);
    }

    [Fact]
    public void Update_Should_UpdateAllFields_And_RaiseWorkshopUpdatedDomainEvent()
    {
        // Arrange
        var workshop = new WorkshopBuilder().Build();
        workshop.ClearDomainEvents();

        var newEmail = "nowy-login@warsztat.pl";
        var newContact = "nowy-kontakt@warsztat.pl";
        var newName = "Nowa Nazwa Serwisu";
        var newDisplay = "NowySerwis";
        var newPhone = "987654321";
        var newAddress = "Nowy Adres 123";
        var newCity = "Nowe Miasto";
        var newPostal = "65-432";
        var newCountry = "Nowy Kraj";

        // Act
        workshop.Update(
            newEmail, newContact, newName, newDisplay, newPhone,
            newAddress, newCity, newPostal, newCountry
        );

        // Assert
        workshop.Email.Should().Be(newEmail);
        workshop.ContactEmail.Should().Be(newContact);
        workshop.Name.Should().Be(newName);
        workshop.DisplayName.Should().Be(newDisplay);
        workshop.PhoneNumber.Should().Be(newPhone);
        workshop.Address.Should().Be(newAddress);
        workshop.City.Should().Be(newCity);
        workshop.PostalCode.Should().Be(newPostal);
        workshop.Country.Should().Be(newCountry);

        workshop.GetDomainEvents().Should().HaveCount(1);
        workshop.GetDomainEvents().First().Should().BeOfType<WorkshopUpdatedDomainEvent>();
    }

    [Theory]
    [InlineData("", "kontakt", "Nazwa", "Display", "123", "Adres", "Miasto", "Kod", "Kraj", "Email cannot be empty.")]
    [InlineData("login", "", "Nazwa", "Display", "123", "Adres", "Miasto", "Kod", "Kraj", "Contact email cannot be empty.")]
    [InlineData("login", "kontakt", "", "Display", "123", "Adres", "Miasto", "Kod", "Kraj", "Name cannot be empty.")]
    [InlineData("login", "kontakt", "Nazwa", "", "123", "Adres", "Miasto", "Kod", "Kraj", "Display name cannot be empty.")]
    [InlineData("login", "kontakt", "Nazwa", "Display", "", "Adres", "Miasto", "Kod", "Kraj", "Phone number cannot be empty.")]
    [InlineData("login", "kontakt", "Nazwa", "Display", "123", "", "Miasto", "Kod", "Kraj", "Address cannot be empty.")]
    [InlineData("login", "kontakt", "Nazwa", "Display", "123", "Adres", "", "Kod", "Kraj", "City cannot be empty.")]
    [InlineData("login", "kontakt", "Nazwa", "Display", "123", "Adres", "Miasto", "", "Kraj", "Postal code cannot be empty.")]
    [InlineData("login", "kontakt", "Nazwa", "Display", "123", "Adres", "Miasto", "Kod", "", "Country cannot be empty.")]
    public void Update_Should_ThrowArgumentException_WhenAnyFieldIsInvalid(
        string email, string contact, string name, string display, string phone,
        string address, string city, string postal, string country, string expectedMessage)
    {
        // Arrange
        var workshop = new WorkshopBuilder().Build();

        workshop.ClearDomainEvents();

        // Act
        Action act = () => workshop.Update(email, contact, name, display, phone, address, city, postal, country);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage($"*{expectedMessage}*");
        workshop.GetDomainEvents().Should().BeEmpty();
    }
}

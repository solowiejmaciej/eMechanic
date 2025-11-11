namespace eMechanic.Domain.Tests.User;

using eMechanic.Domain.User;
using eMechanic.Domain.User.DomainEvents;
using FluentAssertions;

public class UserTests
{
    [Fact]
    public void Create_Should_SuccessfullyCreateUser_WhenDataIsValid()
    {
        // Arrange
        var email = "test@user.pl";
        var firstName = "Test";
        var lastName = "User";
        var identityId = Guid.NewGuid();

        // Act
        var user = User.Create(email, firstName, lastName, identityId);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
        Assert.Equal(firstName, user.FirstName);
        Assert.NotEqual(Guid.Empty, user.Id);
    }

    [Fact]
    public void Create_Should_RaiseUserCreatedDomainEvent_WhenCreated()
    {
        // Act
        var user = User.Create("test@user.pl", "Test", "User", Guid.NewGuid());
        var domainEvents = user.GetDomainEvents();

        // Assert
        Assert.NotNull(domainEvents);
        Assert.Single(domainEvents);
        Assert.IsType<UserCreatedDomainEvent>(domainEvents.First());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ThrowArgumentException_WhenEmailIsEmpty(string invalidEmail)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            User.Create(invalidEmail, "Test", "User", Guid.NewGuid())
        );
    }

    [Fact]
    public void Create_Should_ThrowArgumentException_WhenIdentityIdIsEmpty()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            User.Create("test@user.pl", "Test", "User", Guid.Empty)
        );
    }

    [Fact]
    public void Update_Should_UpdateAllFields_And_RaiseUserUpdatedDomainEvent()
    {
        // Arrange
        var user = CreateValidUser();
        var newEmail = "nowy@email.com";
        var newFirstName = "Jan";
        var newLastName = "Kowalski";

        user.ClearDomainEvents();

        // Act
        user.Update(newEmail, newFirstName, newLastName);

        // Assert
        user.Email.Should().Be(newEmail);
        user.FirstName.Should().Be(newFirstName);
        user.LastName.Should().Be(newLastName);

        var domainEvents = user.GetDomainEvents();
        domainEvents.Should().HaveCount(1);
        domainEvents.First().Should().BeOfType<UserUpdatedDomainEvent>();
    }

    [Fact]
    public void Update_Should_RaiseEvent_EvenIfValuesAreTheSame()
    {
        // Arrange
        var user = CreateValidUser();
        var oldEmail = user.Email;
        var oldFirstName = user.FirstName;
        var oldLastName = user.LastName;

        user.ClearDomainEvents();

        // Act
        user.Update(oldEmail, oldFirstName, oldLastName);

        // Assert
        user.GetDomainEvents().Should().HaveCount(1);
        user.GetDomainEvents().First().Should().BeOfType<UserUpdatedDomainEvent>();
    }

    [Theory]
    [InlineData("", "Jan", "Kowalski")]
    [InlineData("   ", "Jan", "Kowalski")]
    [InlineData("valid@email.com", "", "Kowalski")]
    [InlineData("valid@email.com", "   ", "Kowalski")]
    [InlineData("valid@email.com", "Jan", "")]
    [InlineData("valid@email.com", "Jan", "   ")]
    public void Update_Should_ThrowArgumentException_WhenAnyFieldIsInvalid(string email, string firstName, string lastName)
    {
        // Arrange
        var user = CreateValidUser();

        user.ClearDomainEvents();

        // Act
        Action act = () => user.Update(email, firstName, lastName);

        // Assert
        act.Should().Throw<ArgumentException>();

        user.GetDomainEvents().Should().BeEmpty();
    }

    private User CreateValidUser() => User.Create("test@user.pl", "Test", "User", Guid.NewGuid());
}

namespace eMechanic.Domain.Tests.User;

using eMechanic.Domain.User;
using eMechanic.Domain.User.DomainEvents;

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
}

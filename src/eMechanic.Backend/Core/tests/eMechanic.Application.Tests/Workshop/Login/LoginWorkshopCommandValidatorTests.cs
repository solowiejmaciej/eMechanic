using eMechanic.Application.Workshop.Login;
using FluentValidation.TestHelper;

namespace eMechanic.Application.Tests.Workshop.Login;

public class LoginWorkshopCommandValidatorTests
{
    private readonly LoginWorkshopCommandValidator _validator;

    public LoginWorkshopCommandValidatorTests()
    {
        _validator = new LoginWorkshopCommandValidator();
    }

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValid()
    {
        // Arrange
        var command = new LoginWorkshopCommand("test@workshop.com", "Password123");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_HaveError_WhenEmailIsEmpty(string email)
    {
        // Arrange
        var command = new LoginWorkshopCommand(email, "Password123");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Fact]
    public void Should_HaveError_WhenEmailIsInvalid()
    {
        // Arrange
        var command = new LoginWorkshopCommand("not-an-email", "Password123");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_HaveError_WhenPasswordIsEmpty(string password)
    {
        // Arrange
        var command = new LoginWorkshopCommand("test@workshop.com", password);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }
}

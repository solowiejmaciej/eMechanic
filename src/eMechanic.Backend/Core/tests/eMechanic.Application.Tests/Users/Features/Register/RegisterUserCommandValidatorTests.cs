namespace eMechanic.Application.Tests.Users.Features.Register;

using eMechanic.Application.Users.Features.Register;
using FluentValidation.TestHelper;

public class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _validator;

    public RegisterUserCommandValidatorTests()
    {
        _validator = new RegisterUserCommandValidator();
    }

    [Fact]
    public void Should_HaveError_WhenEmailIsEmpty()
    {
        // Arrange
        var command = new RegisterUserCommand("Jan", "Kowalski", "", "Password123");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Fact]
    public void Should_HaveError_WhenEmailIsInvalid()
    {
        // Arrange
        var command = new RegisterUserCommand("Jan", "Kowalski", "this-is-not-email", "Password123");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Fact]
    public void Should_HaveError_WhenPasswordIsTooShort()
    {
        // Arrange
        var command = new RegisterUserCommand("Jan", "Kowalski", "jan@kowalski.pl", "pass");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValid()
    {
        // Arrange
        var command = new RegisterUserCommand("Jan", "Kowalski", "jan@kowalski.pl", "Password123");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

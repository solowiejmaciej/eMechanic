namespace eMechanic.Application.Tests.Users.Features.Update;

using eMechanic.Application.Users.Features.Update;
using FluentValidation.TestHelper;

public class UpdateUserCommandValidatorTests
{
    private readonly UpdateUserCommandValidator _validator = new();

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValid()
    {
        // Arrange
        var command = new UpdateUserCommand("Jan", "Kowalski", "jan@kowalski.pl");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_HaveError_WhenFirstNameIsEmpty(string invalidName)
    {
        // Arrange
        var command = new UpdateUserCommand(invalidName, "Kowalski", "jan@kowalski.pl");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name is required");
    }

    [Fact]
    public void Should_HaveError_WhenFirstNameIsTooLong()
    {
        // Arrange
        var longName = new string('a', 101);
        var command = new UpdateUserCommand(longName, "Kowalski", "jan@kowalski.pl");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name cannot exceed 100 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_HaveError_WhenLastNameIsEmpty(string invalidName)
    {
        // Arrange
        var command = new UpdateUserCommand("Jan", invalidName, "jan@kowalski.pl");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name is required");
    }

    [Fact]
    public void Should_HaveError_WhenLastNameIsTooLong()
    {
        // Arrange
        var longName = new string('a', 101);
        var command = new UpdateUserCommand("Jan", longName, "jan@kowalski.pl");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name cannot exceed 100 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_HaveError_WhenEmailIsEmpty(string invalidEmail)
    {
        // Arrange
        var command = new UpdateUserCommand("Jan", "Kowalski", invalidEmail);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_HaveError_WhenEmailIsInvalidFormat()
    {
        // Arrange
        var command = new UpdateUserCommand("Jan", "Kowalski", "not-an-email");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("A valid email is required");
    }
}

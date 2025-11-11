namespace eMechanic.Application.Tests.Tokens.Features.Create;

using eMechanic.Application.Token.Features.Create.Workshop;
using FluentValidation.TestHelper;

public class CreateWorkshopTokenCommandValidatorTests
{
    private readonly CreateWorkshopTokenCommandValidator _validator;

    public CreateWorkshopTokenCommandValidatorTests()
    {
        _validator = new CreateWorkshopTokenCommandValidator();
    }

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateWorkshopTokenCommand("test@workshop.com", "Password123");

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
        var command = new CreateWorkshopTokenCommand(email, "Password123");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Fact]
    public void Should_HaveError_WhenEmailIsInvalid()
    {
        // Arrange
        var command = new CreateWorkshopTokenCommand("not-an-email", "Password123");

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
        var command = new CreateWorkshopTokenCommand("test@workshop.com", password);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }
}

namespace eMechanic.Application.Tests.Tokens.Features.Create;

using Application.Tests.Builders;
using eMechanic.Application.Token.Features.Create.User;
using FluentValidation.TestHelper;

public class CreateUserTokenValidatorTests
{
    private readonly CreateUserTokenValidator _validator;

    public CreateUserTokenValidatorTests()
    {
        _validator = new CreateUserTokenValidator();
    }

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateUserTokenCommandBuilder().Build();

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
        var command = new CreateUserTokenCommandBuilder().WithEmail(email).Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Fact]
    public void Should_HaveError_WhenEmailIsInvalid()
    {
        // Arrange
        var command = new CreateUserTokenCommandBuilder().WithEmail("not-an-email").Build();

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
        var command = new CreateUserTokenCommandBuilder().WithPassword(password).Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }
}

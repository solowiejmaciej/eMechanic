namespace eMechanic.Application.Tests.Users.Features.Create;

using Application.Tests.Builders;
using eMechanic.Application.Users.Features.Create;
using FluentValidation.TestHelper;

public class CreateUserCommandValidatorTests
{
    private readonly CreateUserCommandValidator _validator;

    public CreateUserCommandValidatorTests()
    {
        _validator = new CreateUserCommandValidator();
    }

    [Fact]
    public void Should_HaveError_WhenEmailIsEmpty()
    {
        // Arrange
        var command = new CreateUserCommandBuilder().WithEmail("").Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Fact]
    public void Should_HaveError_WhenEmailIsInvalid()
    {
        // Arrange
        var command = new CreateUserCommandBuilder().WithEmail("this-is-not-email").Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Fact]
    public void Should_HaveError_WhenPasswordIsTooShort()
    {
        // Arrange
        var command = new CreateUserCommandBuilder().WithPassword("pass").Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateUserCommandBuilder().Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

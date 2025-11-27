namespace eMechanic.Application.Tests.Tokens.Features.Create;

using eMechanic.Application.Token.Features.Create.User.External;
using FluentValidation.TestHelper;
using Xunit;

public class CreateUserTokenFromGoogleCommandValidatorTests
{
    private readonly CreateUserTokenFromGoogleCommandValidator _validator;

    public CreateUserTokenFromGoogleCommandValidatorTests()
    {
        _validator = new CreateUserTokenFromGoogleCommandValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_HaveError_When_IdTokenIsEmpty(string? idToken)
    {
        var command = new CreateUserTokenFromGoogleCommand(idToken!);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.IdToken);
    }

    [Fact]
    public void Should_NotHaveError_When_IdTokenIsSpecified()
    {
        var command = new CreateUserTokenFromGoogleCommand("valid_token_string");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.IdToken);
    }
}

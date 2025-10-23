namespace eMechanic.Application.Users.Register;

using Common.CQRS;
using FluentValidation;

public sealed record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password) : IResultCommand<Guid>;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(50)
            .WithMessage("First name cannot exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(50)
            .WithMessage("Last name cannot exceed 50 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("A valid email is required")
            .MaximumLength(100)
            .WithMessage("Email cannot exceed 100 characters");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .MaximumLength(100)
            .WithMessage("Password cannot exceed 100 characters");
    }
}


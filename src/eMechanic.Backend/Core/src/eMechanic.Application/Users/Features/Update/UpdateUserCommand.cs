namespace eMechanic.Application.Users.Features.Update;

using Common.Cache.Attributes;
using eMechanic.Common.Cache;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using FluentValidation;
using Get.Current;

[InvalidatesCache(typeof(GetCurrentUserQuery))]
public sealed record UpdateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber = null) : IResultCommand<Success>;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(100)
            .WithMessage("First name cannot exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(100)
            .WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("A valid email is required")
            .MaximumLength(255)
            .WithMessage("Email cannot exceed 255 characters");

        When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber), () =>
        {
            RuleFor(x => x.PhoneNumber!)
                .MaximumLength(20)
                .WithMessage("Phone number cannot exceed 20 characters");
        });
    }
}

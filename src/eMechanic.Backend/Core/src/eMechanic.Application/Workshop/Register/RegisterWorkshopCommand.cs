namespace eMechanic.Application.Workshop.Register;

using Common.CQRS;
using FluentValidation;

public sealed record RegisterWorkshopCommand(
    string Email,
    string Password,
    string ContactEmail,
    string Name,
    string DisplayName,
    string PhoneNumber,
    string Address,
    string City,
    string PostalCode,
    string Country) : IResultCommand<Guid>;


public class RegisterWorkshopCommandValidator : AbstractValidator<RegisterWorkshopCommand>
{
    public RegisterWorkshopCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email (login) cannot be empty.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password cannot be empty.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.");

        RuleFor(x => x.ContactEmail)
            .NotEmpty().WithMessage("Contact email cannot be empty.")
            .EmailAddress().WithMessage("A valid contact email address is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty.");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Display name cannot be empty.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number cannot be empty.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address cannot be empty.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City cannot be empty.");

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Postal code cannot be empty.");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country cannot be empty.");
    }
}

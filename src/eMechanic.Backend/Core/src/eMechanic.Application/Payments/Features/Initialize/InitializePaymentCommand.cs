namespace eMechanic.Application.Payments.Features.Initialize;

using Common;
using eMechanic.Common.CQRS;
using FluentValidation;

public sealed record InitializePaymentCommand(
    Guid ReferenceId,
    EPayableType Type,
    string SuccessUrl,
    string CancelUrl) : IResultCommand<PaymentSessionDto>;

public sealed class InitializePaymentCommandValidator : AbstractValidator<InitializePaymentCommand>
{
    public InitializePaymentCommandValidator()
    {
        RuleFor(x => x.ReferenceId).NotEmpty();

        RuleFor(x => x.SuccessUrl)
            .NotEmpty()
            .Must(BeAbsoluteHttpUrl)
            .WithMessage("SuccessUrl must be an absolute HTTP/HTTPS URL.");

        RuleFor(x => x.CancelUrl)
            .NotEmpty()
            .Must(BeAbsoluteHttpUrl)
            .WithMessage("CancelUrl must be an absolute HTTP/HTTPS URL.");
    }

    private static bool BeAbsoluteHttpUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
               && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp);
    }
}

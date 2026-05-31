namespace eMechanic.Application.Payments.Features.ProcessWebhook;

using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using FluentValidation;

public sealed record ProcessPaymentWebhookCommand(
    string JsonPayload,
    string SignatureHeader) : IResultCommand<Success>;

public sealed class ProcessPaymentWebhookCommandValidator
    : AbstractValidator<ProcessPaymentWebhookCommand>
{
    public ProcessPaymentWebhookCommandValidator()
    {
        RuleFor(x => x.JsonPayload).NotEmpty();
        RuleFor(x => x.SignatureHeader).NotEmpty();
    }
}


namespace eMechanic.Application.Payments.Features.Process;

using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using FluentValidation;

public sealed record ProcessPaymentCommand(
    string JsonPayload,
    string SignatureHeader) : IResultCommand<Success>;

public sealed class ProcessPaymentCommandValidator
    : AbstractValidator<ProcessPaymentCommand>
{
    public ProcessPaymentCommandValidator()
    {
        RuleFor(x => x.JsonPayload).NotEmpty();
        RuleFor(x => x.SignatureHeader).NotEmpty();
    }
}

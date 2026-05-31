namespace eMechanic.Application.Repair.Features.Pay;

using Common.CQRS;
using Common.Result;
using FluentValidation;

public sealed record PayForRepairCommand(Guid RepairId) : IResultCommand<Success>;

public sealed class PayForRepairCommandValidator : AbstractValidator<PayForRepairCommand>
{
    public PayForRepairCommandValidator()
    {
        RuleFor(x => x.RepairId).NotEmpty();
    }
}


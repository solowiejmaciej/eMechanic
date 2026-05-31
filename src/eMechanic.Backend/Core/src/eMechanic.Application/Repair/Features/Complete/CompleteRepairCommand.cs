namespace eMechanic.Application.Repair.Features.Complete;

using Common.CQRS;
using Common.Result;
using FluentValidation;

public sealed record CompleteRepairCommand(Guid RepairId, decimal Amount, string Currency) : IResultCommand<Success>;

public sealed class CompleteRepairCommandValidator : AbstractValidator<CompleteRepairCommand>
{
    public CompleteRepairCommandValidator()
    {
        RuleFor(x => x.RepairId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
    }
}


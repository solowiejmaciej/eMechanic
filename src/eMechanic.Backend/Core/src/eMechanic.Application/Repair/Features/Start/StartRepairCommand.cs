namespace eMechanic.Application.Repair.Features.Start;

using Common.CQRS;
using Common.Result;
using FluentValidation;

public sealed record StartRepairCommand(Guid RepairId) : IResultCommand<Success>;

public sealed class StartRepairCommandValidator : AbstractValidator<StartRepairCommand>
{
    public StartRepairCommandValidator()
    {
        RuleFor(x => x.RepairId).NotEmpty();
    }
}


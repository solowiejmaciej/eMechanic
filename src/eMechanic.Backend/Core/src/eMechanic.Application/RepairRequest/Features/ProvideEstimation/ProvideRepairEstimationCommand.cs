
namespace eMechanic.Application.RepairRequest.Features.ProvideEstimation;

using Common.CQRS;
using Common.Result;
using FluentValidation;

public sealed record ProvideRepairEstimationCommand(Guid RepairRequestId, string Diagnosis, decimal Cost, string Currency) : IResultCommand<Success>;

public class ProvideRepairEstimationValidator : AbstractValidator<ProvideRepairEstimationCommand>
{
    public ProvideRepairEstimationValidator()
    {
        RuleFor(x => x.RepairRequestId).NotEmpty();
        RuleFor(x => x.Diagnosis).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Cost).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
    }
}


namespace eMechanic.Application.RepairRequest.Features.Reject;

using Common.CQRS;
using Common.Result;
using FluentValidation;

public sealed record RejectRepairEstimationCommand(Guid RepairRequestId, string Reason) : IResultCommand<Success>;

public class RejectRepairEstimationValidator : AbstractValidator<RejectRepairEstimationCommand>
{
    public RejectRepairEstimationValidator()
    {
        RuleFor(x => x.RepairRequestId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}

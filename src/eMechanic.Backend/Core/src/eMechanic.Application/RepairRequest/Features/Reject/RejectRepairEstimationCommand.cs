
namespace eMechanic.Application.RepairRequest.Features.Reject;

using Common.Cache;
using Common.Cache.Attributes;
using Common.CQRS;
using Common.Result;
using FluentValidation;
using Get.ForUser;
using Get.ForWorkshop;

[InvalidatesCache(typeof(GetRepairRequestsForUserVehicleQuery), typeof(GetRepairRequestsForWorkshopQuery))]
public sealed record RejectRepairEstimationCommand(Guid RepairRequestId, string Reason) : IResultCommand<Success>;

public class RejectRepairEstimationValidator : AbstractValidator<RejectRepairEstimationCommand>
{
    public RejectRepairEstimationValidator()
    {
        RuleFor(x => x.RepairRequestId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}

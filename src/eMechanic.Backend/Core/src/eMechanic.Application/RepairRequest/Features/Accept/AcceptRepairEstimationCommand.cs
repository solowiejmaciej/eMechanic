
namespace eMechanic.Application.RepairRequest.Features.Accept;

using Common.Cache;
using Common.Cache.Attributes;
using Common.CQRS;
using Common.Result;
using FluentValidation;
using MediatR;
using Get.ForUser;
using Get.ForWorkshop;
using eMechanic.Application.Repair.Features.Get.ForUser;
using eMechanic.Application.Repair.Features.Get.ForWorkshop;

[InvalidatesCache(
    typeof(GetRepairRequestsForUserVehicleQuery),
    typeof(GetRepairRequestsForWorkshopQuery),
    typeof(GetRepairsForUserQuery),
    typeof(GetRepairsForWorkshopQuery))]
public sealed record AcceptRepairEstimationCommand(Guid RepairRequestId) : IResultCommand<Success>;

public class AcceptRepairEstimationValidator : AbstractValidator<AcceptRepairEstimationCommand>
{
    public AcceptRepairEstimationValidator()
    {
        RuleFor(x => x.RepairRequestId).NotEmpty();
    }
}

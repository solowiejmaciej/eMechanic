
namespace eMechanic.Application.RepairRequest.Features.Create;

using Common.Cache;
using Common.Cache.Attributes;
using Common.CQRS;
using FluentValidation;
using Get.ForUser;
using Get.ForWorkshop;

[InvalidatesCache(typeof(GetRepairRequestsForUserVehicleQuery), typeof(GetRepairRequestsForWorkshopQuery))]
public sealed record CreateRepairRequestCommand(Guid VehicleId, Guid WorkshopId, string Description) : IResultCommand<Guid>;

public class CreateRepairRequestValidator : AbstractValidator<CreateRepairRequestCommand>
{
    public CreateRepairRequestValidator()
    {
        RuleFor(x => x.VehicleId).NotEmpty();
        RuleFor(x => x.WorkshopId).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
    }
}

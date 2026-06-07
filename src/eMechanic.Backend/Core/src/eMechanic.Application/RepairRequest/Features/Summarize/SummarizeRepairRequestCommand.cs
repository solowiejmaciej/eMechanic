namespace eMechanic.Application.RepairRequest.Features.Summarize;

using Common.Cache;
using Common.Cache.Attributes;
using Common.CQRS;
using FluentValidation;
using Get.ForUser;
using Get.ForWorkshop;

[InvalidatesCache(typeof(GetRepairRequestsForUserVehicleQuery), typeof(GetRepairRequestsForWorkshopQuery))]
public sealed record SummarizeRepairRequestCommand(Guid RepairRequestId) : IResultCommand<string>;

public class SummarizeRepairRequestValidator : AbstractValidator<SummarizeRepairRequestCommand>
{
    public SummarizeRepairRequestValidator()
    {
        RuleFor(x => x.RepairRequestId).NotEmpty();
    }
}

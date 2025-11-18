
namespace eMechanic.Application.RepairRequest.Features.Get.ForUser;

using Common.CQRS;
using Common.Result;
using FluentValidation;

public sealed record GetRepairRequestsForUserVehicleQuery(Guid VehicleId, PaginationParameters Pagination) : IResultQuery<PaginationResult<RepairRequestResponse>>;

public class GetRepairRequestsForUserVehicleQueryValidator : AbstractValidator<GetRepairRequestsForUserVehicleQuery>
{
    public GetRepairRequestsForUserVehicleQueryValidator()
    {
        RuleFor(x => x.VehicleId).NotEmpty();
        RuleFor(x => x.Pagination.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.Pagination.PageSize).GreaterThanOrEqualTo(1);
    }
}

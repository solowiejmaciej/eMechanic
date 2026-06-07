
namespace eMechanic.Application.RepairRequest.Features.Get.ForUser;

using Common.Cache;
using Common.Cache.Attributes;
using Common.Cache.Configuration;
using Common.CQRS;
using Common.Result;
using FluentValidation;

[Cache(CacheDefaults.DEFAULT_DURATION_SECONDS, ECacheScope.User)]
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

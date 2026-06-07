namespace eMechanic.Application.Vehicle.Timeline.Features.Get;

using Common.Cache.Attributes;
using Common.Cache.Configuration;
using eMechanic.Common.Cache;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using FluentValidation;

[Cache(CacheDefaults.DEFAULT_DURATION_SECONDS, ECacheScope.User)]
public sealed record GetVehicleTimelineByVehicleIdQuery(Guid VehicleId, PaginationParameters PaginationParameters) : IResultQuery<PaginationResult<VehicleTimelineResponse>>;

public sealed class GetVehicleTimelineByVehicleIdQueryValidator : AbstractValidator<GetVehicleTimelineByVehicleIdQuery>
{
    public GetVehicleTimelineByVehicleIdQueryValidator()
    {
        RuleFor(x => x.VehicleId).NotEmpty();
        RuleFor(x => x.PaginationParameters.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PaginationParameters.PageSize).GreaterThanOrEqualTo(1);
    }
}

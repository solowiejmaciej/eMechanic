namespace eMechanic.Application.Vehicle.Features.Get.Timeline;

using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using FluentValidation;

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

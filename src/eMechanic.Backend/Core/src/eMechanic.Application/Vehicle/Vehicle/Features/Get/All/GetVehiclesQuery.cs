namespace eMechanic.Application.Vehicle.Vehicle.Features.Get.All;

using Common.Cache.Attributes;
using Common.Cache.Configuration;
using eMechanic.Common.Cache;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using FluentValidation;

[Cache(CacheDefaults.DEFAULT_DURATION_SECONDS, ECacheScope.User)]
public sealed record GetVehiclesQuery(PaginationParameters PaginationParameters) : IResultQuery<PaginationResult<VehicleResponse>>
{

}

public class GetVehiclesQueryValidator : AbstractValidator<GetVehiclesQuery>
{
    public GetVehiclesQueryValidator()
    {
        RuleFor(x => x.PaginationParameters.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PaginationParameters.PageSize).GreaterThanOrEqualTo(1);
    }
}

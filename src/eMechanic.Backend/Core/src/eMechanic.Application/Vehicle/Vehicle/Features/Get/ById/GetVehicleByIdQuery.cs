namespace eMechanic.Application.Vehicle.Vehicle.Features.Get.ById;

using Common.Cache.Attributes;
using Common.Cache.Configuration;
using eMechanic.Common.Cache;
using eMechanic.Common.CQRS;
using FluentValidation;

[Cache(CacheDefaults.DEFAULT_DURATION_SECONDS, ECacheScope.User)]
public sealed record GetVehicleByIdQuery(Guid Id) : IResultQuery<VehicleResponse>;

public class GetVehicleByIdQueryValidator : AbstractValidator<GetVehicleByIdQuery>
{
    public GetVehicleByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().NotEqual(Guid.Empty);
    }
}

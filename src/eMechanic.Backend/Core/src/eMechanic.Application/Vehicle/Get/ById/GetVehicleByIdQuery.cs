namespace eMechanic.Application.Vehicle.Get.ById;

using eMechanic.Common.CQRS;
using FluentValidation;

public sealed record GetVehicleByIdQuery(Guid Id) : IResultQuery<VehicleResponse>;

public class GetVehicleByIdQueryValidator : AbstractValidator<GetVehicleByIdQuery>
{
    public GetVehicleByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().NotEqual(Guid.Empty);
    }
}

namespace eMechanic.Application.Vehicle.Features.Get.All;

using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using FluentValidation;

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

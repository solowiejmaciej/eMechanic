namespace eMechanic.Application.Vehicle.Document.Features.Get.All;

using Common.Cache.Attributes;
using Common.Cache.Configuration;
using eMechanic.Common.Cache;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using FluentValidation;

[Cache(CacheDefaults.DEFAULT_DURATION_SECONDS, ECacheScope.User)]
public sealed record GetVehicleDocumentsQuery(
    Guid VehicleId,
    PaginationParameters PaginationParameters) : IResultQuery<PaginationResult<VehicleDocumentResponse>>;

public class GetVehicleDocumentsQueryValidator : AbstractValidator<GetVehicleDocumentsQuery>
{
    public GetVehicleDocumentsQueryValidator()
    {
        RuleFor(x => x.VehicleId).NotEmpty();
        RuleFor(x => x.PaginationParameters.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PaginationParameters.PageSize).GreaterThanOrEqualTo(1);
    }
}

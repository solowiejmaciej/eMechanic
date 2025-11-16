namespace eMechanic.Application.VehicleDocument.Features.Get.All;

using Common.CQRS;
using Common.Result;
using FluentValidation;

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

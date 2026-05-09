namespace eMechanic.Application.Vehicle.Document.Features.Get.ById;

using eMechanic.Application.Storage.Dtos;
using eMechanic.Common.CQRS;
using FluentValidation;

public sealed record GetVehicleDocumentFileQuery(
    Guid VehicleId,
    Guid DocumentId) : IResultQuery<FileDownloadResult>;

public class GetVehicleDocumentFileQueryValidator : AbstractValidator<GetVehicleDocumentFileQuery>
{
    public GetVehicleDocumentFileQueryValidator()
    {
        RuleFor(x => x.VehicleId).NotEmpty();
        RuleFor(x => x.DocumentId).NotEmpty();
    }
}

namespace eMechanic.Application.VehicleDocument.Features.Get.ById;

using Abstractions.Storage;
using Common.CQRS;
using FluentValidation;
using Storage.Dtos;

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

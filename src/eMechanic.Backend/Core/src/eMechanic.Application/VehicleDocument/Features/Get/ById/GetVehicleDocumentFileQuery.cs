namespace eMechanic.Application.VehicleDocument.Features.Get.ById;

using Abstractions.Storage;
using Common.CQRS;
using Storage.Dtos;

public sealed record GetVehicleDocumentFileQuery(
    Guid VehicleId,
    Guid DocumentId) : IResultQuery<FileDownloadResult>;

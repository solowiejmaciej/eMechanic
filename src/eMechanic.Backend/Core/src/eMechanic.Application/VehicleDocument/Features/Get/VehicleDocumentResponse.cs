namespace eMechanic.Application.VehicleDocument.Features.Get;

using eMechanic.Domain.VehicleDocument.Enums;

public sealed record VehicleDocumentResponse(
    Guid DocumentId,
    string OriginalFileName,
    EVehicleDocumentType DocumentType,
    string ContentType,
    DateTime CreatedAt);

namespace eMechanic.Application.Vehicle.Document.Features.Get;

using Domain.Vehicle.Documents.Enums;

public sealed record VehicleDocumentResponse(
    Guid DocumentId,
    string OriginalFileName,
    EVehicleDocumentType DocumentType,
    string ContentType,
    DateTime CreatedAt);

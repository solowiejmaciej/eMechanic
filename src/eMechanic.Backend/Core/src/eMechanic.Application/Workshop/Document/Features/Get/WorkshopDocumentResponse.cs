namespace eMechanic.Application.Workshop.Document.Features.Get;

using Domain.Workshop.Documents.Enums;

public record WorkshopDocumentResponse(Guid Id, Uri PublicUrl, string FileName, EWorkshopDocumentType Type);

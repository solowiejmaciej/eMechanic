namespace eMechanic.Domain.Workshop.Documents;

using Common.DDD;
using Common.Result;
using Enums;
using Shared.References.Workshop;

public class WorkshopDocument : AggregateRoot, IWorkshopReference
{
    public Guid WorkshopId { get; private set; }
    public string FullPath { get; private set; }
    public string FileName { get; private set; }
    public EWorkshopDocumentType DocumentType { get; private set; }

    private WorkshopDocument() { }

    private WorkshopDocument(
        Guid id,
        Guid workshopId,
        string fullPath,
        string fileName,
        EWorkshopDocumentType documentType) : base(id)
    {
        WorkshopId = workshopId;
        FullPath = fullPath;
        FileName = fileName;
        DocumentType = documentType;
    }

    public static Result<WorkshopDocument, Error> Create(
        Guid id,
        Guid workshopId,
        Uri fullPath,
        string fileName,
        EWorkshopDocumentType documentType)
    {
        if (id == Guid.Empty)
        {
            return new Error(EErrorCode.ValidationError, "Id cannot be empty");
        }

        if (workshopId == Guid.Empty)
        {
            return new Error(EErrorCode.ValidationError, "WorkshopId cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(fullPath.ToString()))
        {
            return new Error(EErrorCode.ValidationError, "File path cannot be empty");
        }

        if (documentType == EWorkshopDocumentType.None)
        {
            return new Error(EErrorCode.ValidationError, "Invalid document type");
        }

        return new WorkshopDocument(id, workshopId, fullPath.ToString(), fileName, documentType);
    }
}

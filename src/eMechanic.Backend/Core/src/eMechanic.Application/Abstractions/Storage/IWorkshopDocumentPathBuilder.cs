namespace eMechanic.Application.Abstractions.Storage;

public interface IWorkshopDocumentPathBuilder
{
    Uri BuildNewDocumentPath(Guid workshopId, Guid documentId, string originalFileName);
}

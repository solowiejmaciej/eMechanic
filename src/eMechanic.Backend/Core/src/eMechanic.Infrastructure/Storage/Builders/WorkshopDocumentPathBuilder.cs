namespace eMechanic.Infrastructure.Storage.Builders;

using Application.Abstractions.Storage;

internal sealed class WorkshopDocumentPathBuilder : IWorkshopDocumentPathBuilder
{
    private const string CONTAINER_NAME = "workshop-public-documents";

    public Uri BuildNewDocumentPath(Guid workshopId, Guid documentId, string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);

        return new Uri($"{CONTAINER_NAME}/{workshopId}/{documentId}{extension}", UriKind.Relative);
    }
}

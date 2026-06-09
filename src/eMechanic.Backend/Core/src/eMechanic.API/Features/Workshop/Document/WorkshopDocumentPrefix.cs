namespace eMechanic.API.Features.Workshop.Document;

using Vehicle;

public static class WorkshopDocumentPrefix
{
    public const string ENDPOINT = WorkshopPrefix.ENDPOINT + "/documents";
    public const string TAG = "Workshop Documents";
    public const string DELETE = ENDPOINT + "/{documentId:guid}";
    public const string GET_ALL = WorkshopPrefix.ENDPOINT + "/{workshopId:guid}/documents";
    public const string DOWNLOAD = GET_ALL + "/{documentId:guid}/download";
}

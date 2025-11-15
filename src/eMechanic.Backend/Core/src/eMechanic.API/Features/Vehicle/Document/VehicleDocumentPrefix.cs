namespace eMechanic.API.Features.Vehicle.Document;

public static class VehicleDocumentPrefix
{
    public const string ENDPOINT = VehiclePrefix.ENDPOINT + "/{vehicleId:guid}/documents";
    public const string TAG = "Vehicle Documents";
    public const string GET_BY_ID = ENDPOINT + "/{documentId:guid}";
    public const string DOWNLOAD = ENDPOINT + "/{documentId:guid}/download";
    public const string DELETE = ENDPOINT + "/{documentId:guid}";
}

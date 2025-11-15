namespace eMechanic.Application.Storage.Builders;

using System;
using System.IO;
using eMechanic.Application.Abstractions.Storage;

internal sealed class VehicleDocumentPathBuilder : IVehicleDocumentPathBuilder
{
    private const string CONTAINER_NAME = "vehicle-documents";
    public string GetVehicleDirectoryPath(Guid vehicleId) => $"{CONTAINER_NAME}/{vehicleId}/";

    public string BuildNewDocumentPath(Guid vehicleId, Guid newDocumentId, string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);

        var safeFileName = $"{newDocumentId}{extension}";

        return $"{GetVehicleDirectoryPath(vehicleId)}{safeFileName}";
    }
}

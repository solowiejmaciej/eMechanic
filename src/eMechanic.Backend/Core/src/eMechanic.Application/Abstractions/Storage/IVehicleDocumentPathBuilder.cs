namespace eMechanic.Application.Abstractions.Storage;

public interface IVehicleDocumentPathBuilder
{
    string GetVehicleDirectoryPath(Guid vehicleId);
    string BuildNewDocumentPath(Guid vehicleId, Guid newDocumentId, string originalFileName);}

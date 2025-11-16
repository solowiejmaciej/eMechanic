namespace eMechanic.Application.Tests.Builders;

using eMechanic.Application.VehicleDocument.Features.Delete;

public class DeleteVehicleDocumentCommandBuilder
{
    private Guid _vehicleId = Guid.NewGuid();
    private Guid _documentId = Guid.NewGuid();

    public DeleteVehicleDocumentCommandBuilder WithVehicleId(Guid vehicleId)
    {
        _vehicleId = vehicleId;
        return this;
    }

    public DeleteVehicleDocumentCommandBuilder WithDocumentId(Guid documentId)
    {
        _documentId = documentId;
        return this;
    }

    public DeleteVehicleDocumentCommand Build() => new(_vehicleId, _documentId);
}

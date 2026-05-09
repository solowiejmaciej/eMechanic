
using System;

namespace eMechanic.Application.Tests.Builders.VehicleDocument;

using Application.Vehicle.Document.Features.Get.ById;

public class GetVehicleDocumentFileQueryBuilder
{
    private Guid _vehicleId = Guid.NewGuid();
    private Guid _documentId = Guid.NewGuid();

    public GetVehicleDocumentFileQueryBuilder WithVehicleId(Guid vehicleId)
    {
        _vehicleId = vehicleId;
        return this;
    }

    public GetVehicleDocumentFileQueryBuilder WithDocumentId(Guid documentId)
    {
        _documentId = documentId;
        return this;
    }

    public GetVehicleDocumentFileQuery Build()
    {
        return new GetVehicleDocumentFileQuery(_vehicleId, _documentId);
    }
}

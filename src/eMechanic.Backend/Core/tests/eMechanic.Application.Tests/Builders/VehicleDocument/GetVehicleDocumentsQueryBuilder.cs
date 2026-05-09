
using System;
using eMechanic.Common.Result;

namespace eMechanic.Application.Tests.Builders.VehicleDocument;

using Application.Vehicle.Document.Features.Get.All;

public class GetVehicleDocumentsQueryBuilder
{
    private Guid _vehicleId = Guid.NewGuid();
    private PaginationParameters _pagination = new() { PageNumber = 1, PageSize = 10 };

    public GetVehicleDocumentsQueryBuilder WithVehicleId(Guid vehicleId)
    {
        _vehicleId = vehicleId;
        return this;
    }

    public GetVehicleDocumentsQuery Build()
    {
        return new GetVehicleDocumentsQuery(_vehicleId, _pagination);
    }
}

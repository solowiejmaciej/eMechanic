
using System;
using eMechanic.Application.RepairRequest.Features.Get.ForUser;
using eMechanic.Common.Result;

namespace eMechanic.Application.Tests.Builders.RepairRequest;

public class GetRepairRequestsForUserVehicleQueryBuilder
{
    private Guid _vehicleId = Guid.NewGuid();
    private PaginationParameters _pagination = new() { PageNumber = 1, PageSize = 10 };

    public GetRepairRequestsForUserVehicleQueryBuilder WithVehicleId(Guid vehicleId)
    {
        _vehicleId = vehicleId;
        return this;
    }

    public GetRepairRequestsForUserVehicleQuery Build()
    {
        return new GetRepairRequestsForUserVehicleQuery(_vehicleId, _pagination);
    }
}

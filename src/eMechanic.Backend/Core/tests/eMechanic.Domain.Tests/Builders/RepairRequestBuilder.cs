using System;
using eMechanic.Common.Result;
using eMechanic.Domain.RepairRequest.Enums;

namespace eMechanic.Domain.Tests.Builders;

public class RepairRequestBuilder
{
    private Guid _userId = Guid.NewGuid();
    private Guid _vehicleId = Guid.NewGuid();
    private Guid _workshopId = Guid.NewGuid();
    private string _description = "Test description";
    private ERepairRequestStatus _status = ERepairRequestStatus.Pending;

    public RepairRequestBuilder WithUserId(Guid userId)
    {
        _userId = userId;
        return this;
    }
    
    public RepairRequestBuilder WithVehicleId(Guid vehicleId)
    {
        _vehicleId = vehicleId;
        return this;
    }
    
    public RepairRequestBuilder WithWorkshopId(Guid workshopId)
    {
        _workshopId = workshopId;
        return this;
    }

    public RepairRequestBuilder WithStatus(ERepairRequestStatus status)
    {
        _status = status;
        return this;
    }

    public Domain.RepairRequest.RepairRequest Build()
    {
        var repairRequestResult = BuildResult();
        if (repairRequestResult.HasError())
        {
            throw new InvalidOperationException("Failed to create a valid repair request with the default builder values.");
        }
        return repairRequestResult.Value!;
    }
    
    public Result<Domain.RepairRequest.RepairRequest, Error> BuildResult()
    {
        var repairRequestResult = Domain.RepairRequest.RepairRequest.Create(_userId, _vehicleId, _workshopId, _description);
        if (repairRequestResult.HasError())
        {
            return repairRequestResult;
        }
        var repairRequest = repairRequestResult.Value!;

        if (_status == ERepairRequestStatus.Estimated)
        {
            repairRequest.ProvideEstimation("diagnosis", 100);
        }
        else if (_status == ERepairRequestStatus.Accepted)
        {
            repairRequest.ProvideEstimation("diagnosis", 100);
            repairRequest.AcceptEstimation();
        }
        
        return repairRequest;
    }
}
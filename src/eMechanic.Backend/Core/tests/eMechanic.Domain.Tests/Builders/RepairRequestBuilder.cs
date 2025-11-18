namespace eMechanic.Domain.Tests.Builders;

using System;
using Common.Result;
using Domain.RepairRequest;

public class RepairRequestBuilder
{
    private Guid _vehicleId = Guid.NewGuid();
    private Guid _workshopId = Guid.NewGuid();
    private string _description = "Standard issue with the brakes making squeaky noise.";

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

    public RepairRequestBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public Result<RepairRequest, Error> BuildResult() => RepairRequest.Create(_vehicleId, _workshopId, _description);

    public RepairRequest Build()
    {
        var result = BuildResult();
        if (result.HasError())
        {
            throw new InvalidOperationException($"Builder failed: {result.Error!.Message}");
        }
        return result.Value!;
    }
}

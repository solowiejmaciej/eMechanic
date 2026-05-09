
using System;
using eMechanic.Application.RepairRequest.Features.ProvideEstimation;

namespace eMechanic.Application.Tests.Builders.RepairRequest;

public class ProvideRepairEstimationCommandBuilder
{
    private Guid _repairRequestId = Guid.NewGuid();
    private string _diagnosis = "Test diagnosis";
    private decimal _cost = 100;
    private string _currency = "PLN";

    public ProvideRepairEstimationCommandBuilder WithRepairRequestId(Guid repairRequestId)
    {
        _repairRequestId = repairRequestId;
        return this;
    }

    public ProvideRepairEstimationCommand Build()
    {
        return new ProvideRepairEstimationCommand(_repairRequestId, _diagnosis, _cost, _currency);
    }
}

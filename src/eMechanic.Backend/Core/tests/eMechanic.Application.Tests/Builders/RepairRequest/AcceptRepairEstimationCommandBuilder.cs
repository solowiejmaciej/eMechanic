
using System;
using eMechanic.Application.RepairRequest.Features.Accept;

namespace eMechanic.Application.Tests.Builders.RepairRequest;

public class AcceptRepairEstimationCommandBuilder
{
    private Guid _repairRequestId = Guid.NewGuid();

    public AcceptRepairEstimationCommandBuilder WithRepairRequestId(Guid repairRequestId)
    {
        _repairRequestId = repairRequestId;
        return this;
    }

    public AcceptRepairEstimationCommand Build()
    {
        return new AcceptRepairEstimationCommand(_repairRequestId);
    }
}

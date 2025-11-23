
using System;
using eMechanic.Application.RepairRequest.Features.Reject;

namespace eMechanic.Application.Tests.Builders.RepairRequest;

public class RejectRepairEstimationCommandBuilder
{
    private Guid _repairRequestId = Guid.NewGuid();
    private string _reason = "Test reason";

    public RejectRepairEstimationCommandBuilder WithRepairRequestId(Guid repairRequestId)
    {
        _repairRequestId = repairRequestId;
        return this;
    }

    public RejectRepairEstimationCommand Build()
    {
        return new RejectRepairEstimationCommand(_repairRequestId, _reason);
    }
}

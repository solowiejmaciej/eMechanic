
using System;
using eMechanic.Application.RepairRequest.Features.Summarize;

namespace eMechanic.Application.Tests.Builders.RepairRequest;

public class SummarizeRepairRequestCommandBuilder
{
    private Guid _repairRequestId = Guid.NewGuid();

    public SummarizeRepairRequestCommandBuilder WithRepairRequestId(Guid repairRequestId)
    {
        _repairRequestId = repairRequestId;
        return this;
    }

    public SummarizeRepairRequestCommand Build()
    {
        return new SummarizeRepairRequestCommand(_repairRequestId);
    }
}

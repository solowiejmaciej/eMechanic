
using eMechanic.Application.RepairRequest.Features.Create;

namespace eMechanic.Application.Tests.Builders.RepairRequest;

public class CreateRepairRequestCommandBuilder
{
    private Guid _vehicleId = Guid.NewGuid();
    private Guid _workshopId = Guid.NewGuid();
    private string _description = "Test description";

    public CreateRepairRequestCommandBuilder WithVehicleId(Guid vehicleId)
    {
        _vehicleId = vehicleId;
        return this;
    }

    public CreateRepairRequestCommandBuilder WithWorkshopId(Guid workshopId)
    {
        _workshopId = workshopId;
        return this;
    }

    public CreateRepairRequestCommandBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public CreateRepairRequestCommand Build()
    {
        return new CreateRepairRequestCommand(_vehicleId, _workshopId, _description);
    }
}


namespace eMechanic.Application.Tests.Builders.RepairRequest;

using Application.RepairRequest.Features.Create;

public class CreateRepairRequestCommandBuilder
{
    private Guid _vehicleId = Guid.NewGuid();
    private Guid _workshopId = Guid.NewGuid();
    private string _description = "My car is broken, please fix it.";

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

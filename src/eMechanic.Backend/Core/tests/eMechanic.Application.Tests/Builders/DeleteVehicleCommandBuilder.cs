namespace eMechanic.Application.Tests.Builders;

using eMechanic.Application.Vehicle.Features.Delete;

public class DeleteVehicleCommandBuilder
{
    private Guid _id = Guid.NewGuid();

    public DeleteVehicleCommandBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public DeleteVehicleCommand Build()
    {
        return new DeleteVehicleCommand(_id);
    }
}

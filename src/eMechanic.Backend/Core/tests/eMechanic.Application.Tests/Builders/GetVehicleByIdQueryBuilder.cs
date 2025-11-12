namespace eMechanic.Application.Tests.Builders;

using eMechanic.Application.Vehicle.Features.Get.ById;

public class GetVehicleByIdQueryBuilder
{
    private Guid _id = Guid.NewGuid();

    public GetVehicleByIdQueryBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public GetVehicleByIdQuery Build()
    {
        return new GetVehicleByIdQuery(_id);
    }
}

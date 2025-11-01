namespace eMechanic.Application.Vehicle.Create;

using Abstractions.Identity.Contexts;
using eMechanic.Application.Abstractions.Vehicle;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using eMechanic.Domain.Vehicle;

public sealed class CreateVehicleHandler : IResultCommandHandler<CreateVehicleCommand, Guid>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUserContext _userContext;

    public CreateVehicleHandler(
        IVehicleRepository vehicleRepository,
        IUserContext userContext)
    {
        _vehicleRepository = vehicleRepository;
        _userContext = userContext;
    }

    public async Task<Result<Guid, Error>> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        var currenUserId = _userContext.GetUserId();

        var vehicleResult = Vehicle.Create(
            currenUserId,
            request.Vin,
            request.Manufacturer,
            request.Model,
            request.ProductionYear,
            request.EngineCapacity,
            request.Mileage,
            request.MileageUnit,
            request.FuelType,
            request.BodyType,
            request.VehicleType);

        if (vehicleResult.HasError())
        {
            return vehicleResult.Error!;
        }

        var vehicle = vehicleResult.Value!;

        await _vehicleRepository.AddAsync(vehicle, cancellationToken);
        await _vehicleRepository.SaveChangesAsync(cancellationToken);

        return vehicle.Id;
    }
}

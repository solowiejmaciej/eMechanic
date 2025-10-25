namespace eMechanic.Application.Vehicle.Update;

using Abstractions.Identity.Contexts;
using eMechanic.Application.Abstractions.Vehicle;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;

public sealed class UpdateVehicleHandler : IResultCommandHandler<UpdateVehicleCommand, Success>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUserContext _userContext;

    public UpdateVehicleHandler(IVehicleRepository vehicleRepository, IUserContext userContext)
    {
        _vehicleRepository = vehicleRepository;
        _userContext = userContext;
    }

    public async Task<Result<Success, Error>> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId;
        var vehicle = await _vehicleRepository.GetForUserById(request.Id, currentUserId, cancellationToken);

        if (vehicle is null)
        {
            return new Error(EErrorCode.NotFoundError, $"Vehicle with Id '{request.Id}' not found.");
        }

        var vinResult = vehicle.UpdateVin(request.Vin);
        if (vinResult.HasError()) return vinResult.Error!;

        var manufacturerResult = vehicle.UpdateManufacturer(request.Manufacturer);
        if (manufacturerResult.HasError()) return manufacturerResult.Error!;

        var modelResult = vehicle.UpdateModel(request.Model);
        if (modelResult.HasError()) return modelResult.Error!;

        var yearResult = vehicle.UpdateProductionYear(request.ProductionYear);
        if (yearResult.HasError()) return yearResult.Error!;

        var capacityResult = vehicle.UpdateEngineCapacity(request.EngineCapacity);
        if (capacityResult.HasError()) return capacityResult.Error!;

        var fuelTypeResult = vehicle.ChangeFuelType(request.FuelType);
        if (fuelTypeResult.HasError()) return fuelTypeResult.Error!;

        var bodyTypeResult = vehicle.ChangeBodyType(request.BodyType);
        if (bodyTypeResult.HasError()) return bodyTypeResult.Error!;

        var vehicleTypeResult = vehicle.ChangeVehicleType(request.VehicleType);
        if (vehicleTypeResult.HasError()) return vehicleTypeResult.Error!;

         _vehicleRepository.UpdateAsync(vehicle, cancellationToken);
        await _vehicleRepository.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}

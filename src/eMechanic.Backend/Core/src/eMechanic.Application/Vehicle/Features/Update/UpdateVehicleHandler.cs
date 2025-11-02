namespace eMechanic.Application.Vehicle.Update;

using eMechanic.Application.Abstractions.Vehicle;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;

public sealed class UpdateVehicleHandler : IResultCommandHandler<UpdateVehicleCommand, Success>
{
    private readonly IVehicleOwnershipService _vehicleOwnershipService;
    private readonly IVehicleRepository _vehicleRepository;

    public UpdateVehicleHandler(IVehicleRepository vehicleRepository, IVehicleOwnershipService vehicleOwnershipService)
    {
        _vehicleOwnershipService = vehicleOwnershipService;
        _vehicleRepository = vehicleRepository;
    }

    public async Task<Result<Success, Error>> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicleResult = await _vehicleOwnershipService.GetAndVerifyOwnershipAsync(request.Id, cancellationToken);

        if (vehicleResult.HasError())
        {
            return vehicleResult.Error!;
        }

        var vehicle = vehicleResult.Value!;

        var vinResult = vehicle.UpdateVin(request.Vin);
        if (vinResult.HasError())
        {
            return vinResult.Error!;
        }

        var manufacturerResult = vehicle.UpdateManufacturer(request.Manufacturer);
        if (manufacturerResult.HasError())
        {
            return manufacturerResult.Error!;
        }

        var modelResult = vehicle.UpdateModel(request.Model);
        if (modelResult.HasError())
        {
            return modelResult.Error!;
        }

        var yearResult = vehicle.UpdateProductionYear(request.ProductionYear);
        if (yearResult.HasError())
        {
            return yearResult.Error!;
        }

        var capacityResult = vehicle.UpdateEngineCapacity(request.EngineCapacity);
        if (capacityResult.HasError())
        {
            return capacityResult.Error!;
        }

        var fuelTypeResult = vehicle.ChangeFuelType(request.FuelType);
        if (fuelTypeResult.HasError())
        {
            return fuelTypeResult.Error!;
        }

        var bodyTypeResult = vehicle.ChangeBodyType(request.BodyType);
        if (bodyTypeResult.HasError())
        {
            return bodyTypeResult.Error!;
        }

        var vehicleTypeResult = vehicle.ChangeVehicleType(request.VehicleType);
        if (vehicleTypeResult.HasError())
        {
            return vehicleTypeResult.Error!;
        }

        var millageTypeResult = vehicle.UpdateMileage(request.MillageValue, request.MillageUnit);
        if (millageTypeResult.HasError())
        {
            return millageTypeResult.Error!;
        }

        _vehicleRepository.UpdateAsync(vehicle, cancellationToken);
        await _vehicleRepository.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}

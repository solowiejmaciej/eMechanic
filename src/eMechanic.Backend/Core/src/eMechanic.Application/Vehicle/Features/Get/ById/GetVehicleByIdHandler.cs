namespace eMechanic.Application.Vehicle.Features.Get.ById;

using eMechanic.Application.Abstractions.Vehicle;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using Vehicle.Get;
using Vehicle.Get.ById;

public sealed class GetVehicleByIdHandler : IResultQueryHandler<GetVehicleByIdQuery, VehicleResponse>
{
    private readonly IVehicleOwnershipService _vehicleOwnershipService;

    public GetVehicleByIdHandler( IVehicleOwnershipService vehicleOwnershipService)
    {
        _vehicleOwnershipService = vehicleOwnershipService;
    }

    public async Task<Result<VehicleResponse, Error>> Handle(GetVehicleByIdQuery request, CancellationToken cancellationToken)
    {
        var vehicleResult = await _vehicleOwnershipService.GetAndVerifyOwnershipAsync(request.Id, cancellationToken);

        if (vehicleResult.HasError())
        {
            return vehicleResult.Error!;
        }

        var vehicle = vehicleResult.Value!;

        var response = new VehicleResponse(
            vehicle.Id,
            vehicle.UserId,
            vehicle.Vin.Value,
            vehicle.Manufacturer.Value,
            vehicle.Model.Value,
            vehicle.ProductionYear.Value,
            vehicle.EngineCapacity?.Value,
            vehicle.Mileage.Value,
            vehicle.Mileage.Unit,
            vehicle.FuelType,
            vehicle.BodyType,
            vehicle.VehicleType,
            vehicle.CreatedAt);

        return response;
    }
}

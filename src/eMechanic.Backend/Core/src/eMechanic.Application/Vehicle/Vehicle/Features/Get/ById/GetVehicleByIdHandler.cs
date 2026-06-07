namespace eMechanic.Application.Vehicle.Vehicle.Features.Get.ById;

using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using eMechanic.Application.Users.Repositories;
using Services;

public sealed class GetVehicleByIdHandler : IResultQueryHandler<GetVehicleByIdQuery, VehicleResponse>
{
    private readonly IVehicleOwnershipService _vehicleOwnershipService;
    private readonly IUserRepository _userRepository;

    public GetVehicleByIdHandler(
        IVehicleOwnershipService vehicleOwnershipService,
        IUserRepository userRepository)
    {
        _vehicleOwnershipService = vehicleOwnershipService;
        _userRepository = userRepository;
    }

    public async Task<Result<VehicleResponse, Error>> Handle(GetVehicleByIdQuery request, CancellationToken cancellationToken)
    {
        var vehicleResult = await _vehicleOwnershipService.GetAndVerifyOwnershipAsync(request.Id, cancellationToken);

        if (vehicleResult.HasError())
        {
            return vehicleResult.Error!;
        }

        var vehicle = vehicleResult.Value!;

        var user = await _userRepository.GetByIdAsync(vehicle.UserId, cancellationToken);

        if (user is null)
        {
            return new Error(EErrorCode.NotFoundError, "User not found.");
        }

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
            vehicle.LicensePlate.Value,
            vehicle.HorsePower.Value,
            vehicle.FuelType,
            vehicle.BodyType,
            vehicle.VehicleType,
            vehicle.CreatedAt,
            user.FirstName,
            user.LastName,
            user.Email.Value);

        return response;
    }
}

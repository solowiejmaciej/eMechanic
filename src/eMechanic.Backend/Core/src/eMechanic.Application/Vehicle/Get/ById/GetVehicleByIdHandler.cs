namespace eMechanic.Application.Vehicle.Get.ById;

using eMechanic.Application.Abstractions.Identity.Contexts;
using eMechanic.Application.Abstractions.Vehicle;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;

public sealed class GetVehicleByIdHandler : IResultQueryHandler<GetVehicleByIdQuery, VehicleResponse>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUserContext _userContext;

    public GetVehicleByIdHandler(
        IVehicleRepository vehicleRepository,
        IUserContext userContext)
    {
        _vehicleRepository = vehicleRepository;
        _userContext = userContext;
    }

    public async Task<Result<VehicleResponse, Error>> Handle(GetVehicleByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId;
        var vehicle = await _vehicleRepository.GetForUserById(request.Id, currentUserId, cancellationToken);

        if (vehicle is null)
        {
            return new Error(EErrorCode.NotFoundError, $"Vehicle with Id '{request.Id}' not found.");
        }

        var response = new VehicleResponse(
            vehicle.Id,
            vehicle.UserId,
            vehicle.Vin.Value,
            vehicle.Manufacturer.Value,
            vehicle.Model.Value,
            vehicle.ProductionYear.Value,
            vehicle.EngineCapacity?.Value,
            vehicle.FuelType,
            vehicle.BodyType,
            vehicle.VehicleType,
            vehicle.CreatedAt);

        return response;
    }
}

namespace eMechanic.Application.Vehicle.Get.All;

using Abstractions.Identity.Contexts;
using Abstractions.Vehicle;
using Common.CQRS;
using Common.Result;

public class GetVehiclesHandler : IResultQueryHandler<GetVehiclesQuery, PaginationResult<VehicleResponse>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUserContext _userContext;

    public GetVehiclesHandler(
        IVehicleRepository vehicleRepository,
        IUserContext userContext)
    {
        _vehicleRepository = vehicleRepository;
        _userContext = userContext;
    }

    public async Task<Result<PaginationResult<VehicleResponse>, Error>> Handle(
        GetVehiclesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;

        var vehicles = await _vehicleRepository.GetForUserPaginatedAsync(request.PaginationParameters, userId, cancellationToken);
        var result = vehicles.MapToDto(x => new VehicleResponse(x.Id, x.UserId, x.Vin.Value, x.Manufacturer.Value, x.Model.Value,
            x.ProductionYear.Value, x.EngineCapacity?.Value, x.FuelType, x.BodyType, x.VehicleType, x.CreatedAt));

        return result;
    }
}

namespace eMechanic.Application.Vehicle.Get.Timeline;

using Abstractions.Vehicle;
using Abstractions.VehicleTimeline;
using Common.CQRS;
using Common.Result;

public class GetVehicleTimelineByVehicleIdHandler : IResultQueryHandler<GetVehicleTimelineByVehicleIdQuery,
    PaginationResult<VehicleTimelineResponse>>
{
    private readonly IVehicleTimelineRepository _vehicleTimelineRepository;
    private readonly IVehicleOwnershipService _vehicleOwnershipService;

    public GetVehicleTimelineByVehicleIdHandler(IVehicleTimelineRepository vehicleTimelineRepository, IVehicleOwnershipService vehicleOwnershipService)
    {
        _vehicleTimelineRepository = vehicleTimelineRepository;
        _vehicleOwnershipService = vehicleOwnershipService;
    }

    public async Task<Result<PaginationResult<VehicleTimelineResponse>, Error>> Handle(
        GetVehicleTimelineByVehicleIdQuery request, CancellationToken cancellationToken)
    {
       var vehicleResult = await  _vehicleOwnershipService.GetAndVerifyOwnershipAsync(request.VehicleId, cancellationToken);

       if (vehicleResult.HasError())
       {
           return vehicleResult.Error!;
       }

       var timelineItems = await _vehicleTimelineRepository.GetByVehicleIdPaginatedAsync(request.VehicleId, request.PaginationParameters, cancellationToken);

       var result = timelineItems.MapToDto(x => new VehicleTimelineResponse(
           x.EventType,
           x.Data,
           x.CreatedAt));

       return result;
    }
}


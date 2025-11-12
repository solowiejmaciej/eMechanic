namespace eMechanic.Application.Tests.Builders;

using eMechanic.Application.Vehicle.Features.Get.Timeline;
using eMechanic.Common.Result;

public class GetVehicleTimelineByVehicleIdQueryBuilder
{
    private Guid _vehicleId = Guid.NewGuid();
    private PaginationParameters _paginationParameters = new PaginationParametersBuilder().Build();

    public GetVehicleTimelineByVehicleIdQueryBuilder WithVehicleId(Guid vehicleId)
    {
        _vehicleId = vehicleId;
        return this;
    }

    public GetVehicleTimelineByVehicleIdQueryBuilder WithPaginationParameters(PaginationParameters paginationParameters)
    {
        _paginationParameters = paginationParameters;
        return this;
    }

    public GetVehicleTimelineByVehicleIdQuery Build()
    {
        return new GetVehicleTimelineByVehicleIdQuery(_vehicleId, _paginationParameters);
    }
}

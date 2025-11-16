namespace eMechanic.Application.Tests.Builders;

using eMechanic.Application.VehicleDocument.Features.Get.All;
using eMechanic.Common.Result;

public class GetVehicleDocumentsQueryBuilder
{
    private Guid _vehicleId = Guid.NewGuid();
    private PaginationParameters _pagination = new PaginationParametersBuilder().Build();

    public GetVehicleDocumentsQueryBuilder WithVehicleId(Guid vehicleId)
    {
        _vehicleId = vehicleId;
        return this;
    }

    public GetVehicleDocumentsQueryBuilder WithPagination(int pageNumber, int pageSize)
    {
        _pagination = new PaginationParametersBuilder()
            .WithPageNumber(pageNumber)
            .WithPageSize(pageSize)
            .Build();
        return this;
    }

    public GetVehicleDocumentsQuery Build() => new(_vehicleId, _pagination);
}

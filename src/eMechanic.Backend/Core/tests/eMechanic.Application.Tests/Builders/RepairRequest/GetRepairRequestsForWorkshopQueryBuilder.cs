
using eMechanic.Application.RepairRequest.Features.Get.ForWorkshop;
using eMechanic.Common.Result;

namespace eMechanic.Application.Tests.Builders.RepairRequest;

public class GetRepairRequestsForWorkshopQueryBuilder
{
    private PaginationParameters _pagination = new() { PageNumber = 1, PageSize = 10 };

    public GetRepairRequestsForWorkshopQuery Build()
    {
        return new GetRepairRequestsForWorkshopQuery(_pagination);
    }
}

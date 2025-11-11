namespace eMechanic.Application.Workshop.Features.Get.All;

using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using Repositories;

public class GetWorkshopsHandler : IResultQueryHandler<GetWorkshopsQuery, PaginationResult<WorkshopResponse>>
{
    private readonly IWorkshopRepository _workshopRepository;

    public GetWorkshopsHandler(IWorkshopRepository workshopRepository)
    {
        _workshopRepository = workshopRepository;
    }

    public async Task<Result<PaginationResult<WorkshopResponse>, Error>> Handle(GetWorkshopsQuery request, CancellationToken cancellationToken)
    {
        var workshops = await _workshopRepository.GetPaginatedAsync(request.PaginationParameters, cancellationToken);
        var result = workshops.MapToDto(x => new WorkshopResponse(x.Id, x.ContactEmail, x.DisplayName, x.PhoneNumber,
            x.Address, x.City, x.PostalCode, x.Country));

        return result;
    }
}

namespace eMechanic.Application.Workshop.Features.Update;

using eMechanic.Application.Abstractions.Identity.Contexts;
using eMechanic.Application.Workshop.Services;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;

public class UpdateWorkshopHandler : IResultCommandHandler<UpdateWorkshopCommand, Success>
{
    private readonly IWorkshopContext _workshopContext;
    private readonly IWorkshopService _workshopService;

    public UpdateWorkshopHandler(
        IWorkshopContext workshopContext,
        IWorkshopService workshopService)
    {
        _workshopContext = workshopContext;
        _workshopService = workshopService;
    }

    public async Task<Result<Success, Error>> Handle(UpdateWorkshopCommand request, CancellationToken cancellationToken)
    {
        var domainWorkshopId = _workshopContext.GetWorkshopId();

        return await _workshopService.UpdateWorkshopWithIdentityAsync(
            domainWorkshopId,
            request.Email,
            request.ContactEmail,
            request.Name,
            request.DisplayName,
            request.PhoneNumber,
            request.Address,
            request.City,
            request.PostalCode,
            request.Country,
            cancellationToken);
    }
}

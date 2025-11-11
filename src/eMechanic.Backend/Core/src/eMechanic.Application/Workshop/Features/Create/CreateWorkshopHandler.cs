namespace eMechanic.Application.Workshop.Features.Create;

using eMechanic.Application.Workshop.Services;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;

public class CreateWorkshopHandler : IResultCommandHandler<CreateWorkshopCommand, Guid>
{
    private readonly IWorkshopService _workshopService;

    public CreateWorkshopHandler(IWorkshopService workshopService)
    {
        _workshopService = workshopService;
    }

    public async Task<Result<Guid, Error>> Handle(CreateWorkshopCommand request, CancellationToken cancellationToken)
    {
        var result = await _workshopService.CreateWorkshopWithIdentityAsync(
            request.Email,
            request.Password,
            request.ContactEmail,
            request.Name,
            request.DisplayName,
            request.PhoneNumber,
            request.Address,
            request.City,
            request.PostalCode,
            request.Country,
            cancellationToken);

        return result;
    }
}

namespace eMechanic.Application.Workshop.Features.Register;

using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using Services;

public class RegisterWorkshopHandler : IResultCommandHandler<RegisterWorkshopCommand, Guid>
{
    private readonly IWorkshopService _workshopService;

    public RegisterWorkshopHandler(IWorkshopService workshopService)
    {
        _workshopService = workshopService;
    }

    public async Task<Result<Guid, Error>> Handle(RegisterWorkshopCommand request, CancellationToken cancellationToken)
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

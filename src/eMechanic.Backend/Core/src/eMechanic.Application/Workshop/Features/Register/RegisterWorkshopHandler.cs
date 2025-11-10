namespace eMechanic.Application.Workshop.Features.Register;

using eMechanic.Application.Abstractions.Workshop;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;

public class RegisterWorkshopHandler : IResultCommandHandler<RegisterWorkshopCommand, Guid>
{
    private readonly IWorkshopCreatorService _workshopCreatorService;

    public RegisterWorkshopHandler(IWorkshopCreatorService workshopCreatorService)
    {
        _workshopCreatorService = workshopCreatorService;
    }

    public async Task<Result<Guid, Error>> Handle(RegisterWorkshopCommand request, CancellationToken cancellationToken)
    {
        var result = await _workshopCreatorService.CreateWorkshopWithIdentityAsync(
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

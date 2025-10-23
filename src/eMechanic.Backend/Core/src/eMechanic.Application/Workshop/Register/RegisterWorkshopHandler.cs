namespace eMechanic.Application.Workshop.Register;

using Abstractions.Workshop;
using Common.CQRS;
using Common.Result;
using Microsoft.Extensions.Logging;

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

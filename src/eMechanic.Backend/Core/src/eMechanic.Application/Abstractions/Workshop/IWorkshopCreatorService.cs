namespace eMechanic.Application.Abstractions.Workshop;

using Common.Result;

public interface IWorkshopCreatorService
{
    Task<Result<Guid, Error>> CreateWorkshopWithIdentityAsync(
        string email,
        string password,
        string contactEmail,
        string name,
        string displayName,
        string phoneNumber,
        string address,
        string city,
        string postalCode,
        string country,
        CancellationToken cancellationToken);
}

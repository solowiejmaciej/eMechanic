namespace eMechanic.Application.Workshop.Services;

using eMechanic.Common.Result;

public interface IWorkshopService
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

    Task<Result<Success, Error>> UpdateWorkshopWithIdentityAsync(
        Guid domainWorkshopId,
        string email,
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

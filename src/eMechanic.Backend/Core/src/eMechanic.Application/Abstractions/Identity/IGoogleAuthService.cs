using eMechanic.Application.Identity;
using eMechanic.Common.Result;

namespace eMechanic.Application.Abstractions.Identity;

public interface IGoogleAuthService
{
    Task<Result<AuthenticatedIdentity, Error>> LoginAsync(string idToken, CancellationToken cancellationToken);
}

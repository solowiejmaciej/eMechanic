namespace eMechanic.Application.Abstractions.Identity;

using Application.Identity;

public interface ITokenGenerator
{
    TokenDTO GenerateToken(AuthenticatedIdentity identity);
}

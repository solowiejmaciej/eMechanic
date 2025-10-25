namespace eMechanic.Application.Abstractions.Identity.Contexts;

public interface IUserContext
{
    Guid UserId { get; }
    bool IsAuthenticated { get; }
}

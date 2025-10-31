namespace eMechanic.Application.Abstractions.Identity.Contexts;

public interface IUserContext
{
    Guid GetUserId();
    bool IsAuthenticated { get; }
}

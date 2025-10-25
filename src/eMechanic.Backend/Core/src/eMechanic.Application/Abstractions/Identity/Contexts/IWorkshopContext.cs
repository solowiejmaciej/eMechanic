namespace eMechanic.Application.Abstractions.Identity.Contexts;

public interface IWorkshopContext
{
    Guid WorkshopId { get; }
    bool IsAuthenticated { get; }
}

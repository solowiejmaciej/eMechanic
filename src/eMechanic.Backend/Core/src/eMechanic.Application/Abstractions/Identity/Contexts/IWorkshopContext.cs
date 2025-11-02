namespace eMechanic.Application.Abstractions.Identity.Contexts;

public interface IWorkshopContext
{
    Guid GetWorkshopId();
    bool IsAuthenticated { get; }
}

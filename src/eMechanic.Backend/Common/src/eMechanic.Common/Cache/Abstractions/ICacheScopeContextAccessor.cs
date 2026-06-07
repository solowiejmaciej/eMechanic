namespace eMechanic.Common.Cache.Abstractions;

public interface ICacheScopeContextAccessor
{
    Guid? GetUserIdOrDefault();
    Guid? GetWorkshopIdOrDefault();
}



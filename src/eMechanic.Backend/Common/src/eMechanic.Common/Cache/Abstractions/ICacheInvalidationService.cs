namespace eMechanic.Common.Cache.Abstractions;

public interface ICacheInvalidationService
{
    Task InvalidateAsync(Type commandType, IReadOnlyCollection<Type> queryTypes, CancellationToken cancellationToken);
}



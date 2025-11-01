namespace eMechanic.Infrastructure.Repositories.Base;

using Application.Abstractions.Repositories;
using Common.DDD;
using Common.Result;
using DAL;
using Microsoft.EntityFrameworkCore;
using Services;
using Specifications;

public class Repository<T> : IRepository<T> where T : Entity
{
    private readonly AppDbContext _context;
    protected readonly DbSet<T> DbSet;
    private readonly IPaginationService _paginationService;

    protected Repository(AppDbContext context, IPaginationService paginationService)
    {
        _context = context;
        _paginationService = paginationService;
        DbSet = context.Set<T>();
    }

    protected IQueryable<T> GetQuery() => DbSet.AsQueryable();
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken) => await DbSet.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<Guid> AddAsync(T entity, CancellationToken cancellationToken)
    {
        var entry = await DbSet.AddAsync(entity, cancellationToken);
        return entry.Entity.Id;
    }

    public void UpdateAsync(T entity, CancellationToken cancellationToken) => _context.Entry(entity).CurrentValues.SetValues(entity);

    public void DeleteAsync(T entity, CancellationToken cancellationToken) => DbSet.Remove(entity);

    protected Task<PaginationResult<T>> GetPaginatedAsync(IQueryable<T> query, PaginationParameters paginationParameters, CancellationToken cancellationToken) => _paginationService.GetPaginatedResultAsync(query, paginationParameters, cancellationToken);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            foreach (var entry in _context.ChangeTracker.Entries<Entity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }

            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException("A concurrency conflict occurred while saving changes", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("An error occurred while saving changes to the database", ex);
        }
    }

}

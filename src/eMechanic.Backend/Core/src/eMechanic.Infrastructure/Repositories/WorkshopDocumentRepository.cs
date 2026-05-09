namespace eMechanic.Infrastructure.Repositories;

using Application.Workshop.Document.Repositories;
using Base;
using Common.Result;
using DAL;
using Domain.Workshop.Documents;
using Extensions;
using Microsoft.EntityFrameworkCore;
using Services;

internal sealed class WorkshopDocumentRepository : Repository<WorkshopDocument>, IWorkshopDocumentRepository
{
    public WorkshopDocumentRepository(AppDbContext context, IPaginationService paginationService)
        : base(context, paginationService)
    {
    }

    public Task<PaginationResult<WorkshopDocument>> GetByWorkshopIdAsync(
        Guid workshopId,
        PaginationParameters paginationParameters,
        CancellationToken cancellationToken)
    {
        var query =
            GetQuery()
                .FilterByWorkshopId(workshopId)
                .OrderByDescending(d => d.CreatedAt);

        return GetPaginatedAsync(query, paginationParameters, cancellationToken);
    }
    
}

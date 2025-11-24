namespace eMechanic.Application.Workshop.Document.Repositories;

using Common.Result;
using eMechanic.Application.Abstractions.Repositories;
using eMechanic.Domain.Workshop.Documents;

public interface IWorkshopDocumentRepository : IRepository<WorkshopDocument>
{
    Task<PaginationResult<WorkshopDocument>> GetByWorkshopIdAsync(Guid workshopId, PaginationParameters paginationParameters, CancellationToken cancellationToken);
}

namespace eMechanic.Application.Workshop.Document.Features.Get;

using Abstractions.Storage;
using Common.CQRS;
using Common.Result;
using Repositories;

public sealed class GetWorkshopDocumentsQueryHandler : IResultQueryHandler<GetWorkshopDocumentsQuery, PaginationResult<WorkshopDocumentResponse>>
{
    private readonly IWorkshopDocumentRepository _repository;
    private readonly IFileStorageService _fileStorage;

    public GetWorkshopDocumentsQueryHandler(IWorkshopDocumentRepository repository, IFileStorageService fileStorage)
    {
        _repository = repository;
        _fileStorage = fileStorage;
    }

    public async Task<Result<PaginationResult<WorkshopDocumentResponse>, Error>> Handle(GetWorkshopDocumentsQuery request, CancellationToken cancellationToken)
    {
        var documents = await _repository.GetByWorkshopIdAsync(request.WorkshopId, request.PaginationParameters, cancellationToken);

        var response = documents.MapToDto(doc => new WorkshopDocumentResponse(
            doc.Id,
            _fileStorage.GetPublicUrl(doc.FullPath),
            doc.FileName,
            doc.DocumentType,
            doc.CreatedAt)
        );

        return response;
    }
}

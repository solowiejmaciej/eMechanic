namespace eMechanic.Application.Workshop.Document.Features.Get;

using eMechanic.Application.Abstractions.Storage;
using eMechanic.Application.Storage.Dtos;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using Microsoft.Extensions.Logging;
using Repositories;

internal sealed class GetWorkshopDocumentFileQueryHandler
    : IResultQueryHandler<GetWorkshopDocumentFileQuery, FileDownloadResult>
{
    private readonly IWorkshopDocumentRepository _documentRepository;
    private readonly IFileStorageService _fileStorage;
    private readonly ILogger<GetWorkshopDocumentFileQueryHandler> _logger;

    public GetWorkshopDocumentFileQueryHandler(
        IWorkshopDocumentRepository documentRepository,
        IFileStorageService fileStorage,
        ILogger<GetWorkshopDocumentFileQueryHandler> logger)
    {
        _documentRepository = documentRepository;
        _fileStorage = fileStorage;
        _logger = logger;
    }

    public async Task<Result<FileDownloadResult, Error>> Handle(
        GetWorkshopDocumentFileQuery request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.DocumentId, cancellationToken);
        if (document is null)
        {
            return new Error(EErrorCode.NotFoundError, "Document doesn't exist");
        }

        if (document.WorkshopId != request.WorkshopId)
        {
            _logger.LogWarning(
                "Unauthorized access workshop {RequestWorkshopId}, but document {DocumentId} belongs to other workshop ({DocumentWorkshopId}).",
                request.WorkshopId,
                request.DocumentId,
                document.WorkshopId);

            throw new UnauthorizedAccessException();
        }

        return await _fileStorage.GetFileAsync(document.FullPath, cancellationToken, document.FileName);
    }
}

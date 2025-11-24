namespace eMechanic.Application.Workshop.Document.Features.Delete;

using eMechanic.Application.Abstractions.Identity.Contexts;
using eMechanic.Application.Abstractions.Storage;
using eMechanic.Application.Workshop.Document.Repositories;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;

public sealed class DeleteWorkshopDocumentCommandHandler : IResultCommandHandler<DeleteWorkshopDocumentCommand, Success>
{
    private readonly IWorkshopContext _workshopContext;
    private readonly IWorkshopDocumentRepository _repository;
    private readonly IFileStorageService _fileStorage;

    public DeleteWorkshopDocumentCommandHandler(
        IWorkshopContext workshopContext,
        IWorkshopDocumentRepository repository,
        IFileStorageService fileStorage)
    {
        _workshopContext = workshopContext;
        _repository = repository;
        _fileStorage = fileStorage;
    }

    public async Task<Result<Success, Error>> Handle(DeleteWorkshopDocumentCommand request, CancellationToken cancellationToken)
    {
        var workshopId = _workshopContext.GetWorkshopId();

        var document = await _repository.GetByIdAsync(request.DocumentId, cancellationToken);
        if (document is null)
        {
            return new Error(EErrorCode.NotFoundError, "Document not found");
        }

        if (document.WorkshopId != workshopId)
        {
            return new Error(EErrorCode.NotFoundError, "Document not found");
        }

        await _fileStorage.DeleteFileAsync(document.FullPath, cancellationToken);

        await _repository.DeleteAsync(document, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}

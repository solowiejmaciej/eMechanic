namespace eMechanic.Application.Workshop.Document.Features.Create;

using eMechanic.Application.Abstractions.Identity.Contexts;
using eMechanic.Application.Abstractions.Storage;
using eMechanic.Application.Workshop.Document.Repositories;
using eMechanic.Common.CQRS;
using eMechanic.Common.Helpers;
using eMechanic.Common.Result;
using eMechanic.Domain.Workshop.Documents;

public class AddWorkshopDocumentCommandHandler : IResultCommandHandler<AddWorkshopDocumentCommand, Uri>
{
    private readonly IWorkshopContext _workshopContext;
    private readonly IWorkshopDocumentPathBuilder _pathBuilder;
    private readonly IFileStorageService _fileStorage;
    private readonly IWorkshopDocumentRepository _repository;

    public AddWorkshopDocumentCommandHandler(
        IWorkshopContext workshopContext,
        IWorkshopDocumentPathBuilder pathBuilder,
        IFileStorageService fileStorage,
        IWorkshopDocumentRepository repository)
    {
        _workshopContext = workshopContext;
        _pathBuilder = pathBuilder;
        _fileStorage = fileStorage;
        _repository = repository;
    }

    public async Task<Result<Uri, Error>> Handle(AddWorkshopDocumentCommand request, CancellationToken cancellationToken)
    {
        var workshopId = _workshopContext.GetWorkshopId();
        var newDocId = GuidFactory.Create();

        var fullPath = _pathBuilder.BuildNewDocumentPath(workshopId, newDocId, request.File.FileName);

        var uploadResult = await _fileStorage.UploadFileAsync(fullPath.ToString(), request.File, cancellationToken);
        if(uploadResult.HasError())
        {
            return uploadResult.Error!;
        }

        var docResult = WorkshopDocument.Create(newDocId, workshopId, fullPath, request.File.FileName, request.DocumentType);
        if(docResult.HasError())
        {
            return docResult.Error!;
        }

        await _repository.AddAsync(docResult.Value!, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return _fileStorage.GetPublicUrl(fullPath.ToString());
    }
}

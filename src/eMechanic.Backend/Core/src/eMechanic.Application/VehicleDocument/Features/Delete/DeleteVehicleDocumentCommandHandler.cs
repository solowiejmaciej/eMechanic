namespace eMechanic.Application.VehicleDocument.Features.Delete;

using Abstractions.Storage;
using Common.CQRS;
using Common.Result;
using Microsoft.Extensions.Logging;
using Repositories;
using Vehicle.Services;

internal sealed class DeleteVehicleDocumentCommandHandler : IResultCommandHandler<DeleteVehicleDocumentCommand, Success>
{
    private readonly IVehicleOwnershipService _ownershipService;
    private readonly IVehicleDocumentRepository _documentRepository;
    private readonly IFileStorageService _fileStorage;
    private readonly ILogger<DeleteVehicleDocumentCommandHandler> _logger;

    public DeleteVehicleDocumentCommandHandler(
        IVehicleOwnershipService ownershipService,
        IVehicleDocumentRepository documentRepository,
        IFileStorageService fileStorage,
        ILogger<DeleteVehicleDocumentCommandHandler> logger)
    {
        _ownershipService = ownershipService;
        _documentRepository = documentRepository;
        _fileStorage = fileStorage;
        _logger = logger;
    }

    public async Task<Result<Success, Error>> Handle(DeleteVehicleDocumentCommand request, CancellationToken cancellationToken)
    {
        var ownershipResult = await _ownershipService.GetAndVerifyOwnershipAsync(request.VehicleId, cancellationToken);
        if (ownershipResult.HasError())
        {
            return ownershipResult.Error!;
        }

        var document = await _documentRepository.GetByIdAsync(request.DocumentId, cancellationToken);
        if (document is null)
        {
            return new Error(EErrorCode.NotFoundError, "Document not found");
        }

        if (document.VehicleId != request.VehicleId)
        {
            _logger.LogWarning(
                "Unauthorized access user has access {VehicleId}, but document {DocumentId} belongs to other vehicle ({DocumentVehicleId}).",
                request.VehicleId,
                request.DocumentId,
                document.VehicleId);

            throw new UnauthorizedAccessException();
        }

        document.RaiseDeletedEvent();
        _documentRepository.DeleteAsync(document, cancellationToken);
        await _documentRepository.SaveChangesAsync(cancellationToken);

        var deleteResult = await _fileStorage.DeleteFileAsync(document.FullPath, cancellationToken);
        if (deleteResult.HasError())
        {
            _logger.LogError("Successfully deleted {DocumentId} from db, but unable to delete {FullPath} from storage {Error}", document.Id, document.FullPath, deleteResult.Error!.Message);
            return deleteResult.Error!;
        }

        return Result.Success;
    }
}

namespace eMechanic.Application.VehicleDocument.Features.Get.ById;

using Abstractions.Storage;
using eMechanic.Application.Vehicle.Services;
using eMechanic.Application.VehicleDocument.Repositories;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using Microsoft.Extensions.Logging;
using Storage.Dtos;

internal sealed class GetVehicleDocumentFileQueryHandler
    : IResultQueryHandler<GetVehicleDocumentFileQuery, FileDownloadResult>
{
    private readonly IVehicleOwnershipService _ownershipService;
    private readonly IVehicleDocumentRepository _documentRepository;
    private readonly IFileStorageService _fileStorage;
    private readonly ILogger<GetVehicleDocumentFileQueryHandler> _logger;

    public GetVehicleDocumentFileQueryHandler(
        IVehicleOwnershipService ownershipService,
        IVehicleDocumentRepository documentRepository,
        IFileStorageService fileStorage,
        ILogger<GetVehicleDocumentFileQueryHandler> logger)
    {
        _ownershipService = ownershipService;
        _documentRepository = documentRepository;
        _fileStorage = fileStorage;
        _logger = logger;
    }

    public async Task<Result<FileDownloadResult, Error>> Handle(
        GetVehicleDocumentFileQuery request, CancellationToken cancellationToken)
    {
        var ownershipResult = await _ownershipService.GetAndVerifyOwnershipAsync(request.VehicleId, cancellationToken);
        if (ownershipResult.HasError())
        {
            return ownershipResult.Error!;
        }

        var document = await _documentRepository.GetByIdAsync(request.DocumentId, cancellationToken);
        if (document is null)
        {
            return new Error(EErrorCode.NotFoundError, "Document doesn't exists");
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

        return await _fileStorage.GetFileAsync(document.FullPath, cancellationToken, document.OriginalFileName);
    }
}

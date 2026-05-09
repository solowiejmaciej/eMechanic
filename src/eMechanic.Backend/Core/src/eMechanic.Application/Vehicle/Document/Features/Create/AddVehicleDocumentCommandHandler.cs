namespace eMechanic.Application.Vehicle.Document.Features.Create;

using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Vehicle.Documents;
using eMechanic.Application.Abstractions.Storage;
using eMechanic.Common.CQRS;
using eMechanic.Common.Helpers;
using eMechanic.Common.Result;
using Repositories;
using Vehicle.Services;

internal sealed class AddVehicleDocumentCommandHandler : IResultCommandHandler<AddVehicleDocumentCommand, Guid>
{
    private readonly IVehicleOwnershipService _ownershipService;
    private readonly IVehicleDocumentRepository _documentRepository;
    private readonly IVehicleDocumentPathBuilder _pathBuilder;
    private readonly IFileStorageService _fileStorage;

    public AddVehicleDocumentCommandHandler(
        IVehicleOwnershipService ownershipService,
        IVehicleDocumentRepository documentRepository,
        IVehicleDocumentPathBuilder pathBuilder,
        IFileStorageService fileStorage)
    {
        _ownershipService = ownershipService;
        _documentRepository = documentRepository;
        _pathBuilder = pathBuilder;
        _fileStorage = fileStorage;
    }

    public async Task<Result<Guid, Error>> Handle(AddVehicleDocumentCommand request, CancellationToken cancellationToken)
    {
        var ownershipResult = await _ownershipService.GetAndVerifyOwnershipAsync(request.VehicleId, cancellationToken);
        if (ownershipResult.HasError())
        {
            return ownershipResult.Error!;
        }

        var newDocumentId = GuidFactory.Create();

        var fullPath = _pathBuilder.BuildNewDocumentPath(request.VehicleId, newDocumentId, request.File.FileName);

        var documentResult = VehicleDocument.Create(
            newDocumentId,
            request.VehicleId,
            fullPath,
            request.File.FileName,
            request.File.ContentType,
            request.DocumentType
        );

        if (documentResult.HasError())
        {
            return documentResult.Error!;
        }

        var document = documentResult.Value!;

        var uploadResult = await _fileStorage.UploadFileAsync(fullPath, request.File, cancellationToken);
        if (uploadResult.HasError())
        {
            return uploadResult.Error!;
        }

        try
        {
            await _documentRepository.AddAsync(document, cancellationToken);
            await _documentRepository.SaveChangesAsync(cancellationToken);
        }
        catch (Exception)
        {
            await _fileStorage.DeleteFileAsync(fullPath, cancellationToken);
            throw;
        }

        return document.Id;
    }
}

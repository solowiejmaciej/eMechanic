namespace eMechanic.Application.VehicleDocument.Features.Get.All;

using eMechanic.Application.Vehicle.Services;
using eMechanic.Application.VehicleDocument.Repositories;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;

internal sealed class GetVehicleDocumentsQueryHandler
    : IResultQueryHandler<GetVehicleDocumentsQuery, PaginationResult<VehicleDocumentResponse>>
{
    private readonly IVehicleOwnershipService _ownershipService;
    private readonly IVehicleDocumentRepository _documentRepository;

    public GetVehicleDocumentsQueryHandler(
        IVehicleOwnershipService ownershipService,
        IVehicleDocumentRepository documentRepository)
    {
        _ownershipService = ownershipService;
        _documentRepository = documentRepository;
    }

    public async Task<Result<PaginationResult<VehicleDocumentResponse>, Error>> Handle(
        GetVehicleDocumentsQuery request, CancellationToken cancellationToken)
    {
        var ownershipResult = await _ownershipService.GetAndVerifyOwnershipAsync(request.VehicleId, cancellationToken);
        if (ownershipResult.HasError())
        {
            return ownershipResult.Error!;
        }

        var documents = await _documentRepository.GetByVehicleIdPaginatedAsync(
            request.VehicleId,
            request.PaginationParameters,
            cancellationToken);

        var result = documents.MapToDto(doc => new VehicleDocumentResponse(
            doc.Id,
            doc.OriginalFileName,
            doc.DocumentType,
            doc.ContentType,
            doc.CreatedAt
        ));

        return result;
    }
}

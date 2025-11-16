namespace eMechanic.Domain.VehicleDocument;

using Common.DDD;
using Common.Result;
using Domain.References.Vehicle;
using Domain.VehicleDocument.Enums;
using DomainEvents;
using Vehicle.Documents.DomainEvents;

public class VehicleDocument : AggregateRoot, IVehicleReference
{
    public Guid VehicleId { get; private set; }
    public string FullPath { get; private set; }
    public string OriginalFileName { get; private set; }
    public string ContentType { get; private set; }
    public EVehicleDocumentType DocumentType { get; private set; }

    private VehicleDocument() { }

    private VehicleDocument(
        Guid id,
        Guid vehicleId,
        string fullPath,
        string originalFileName,
        string contentType,
        EVehicleDocumentType documentType) : base(id)
    {
        VehicleId = vehicleId;
        FullPath = fullPath;
        OriginalFileName = originalFileName;
        ContentType = contentType;
        DocumentType = documentType;
    }

    public static Result<VehicleDocument, Error> Create(
        Guid id,
        Guid vehicleId,
        string fullPath,
        string originalFileName,
        string contentType,
        EVehicleDocumentType documentType)
    {
        if (id == Guid.Empty)
        {
            return new Error(EErrorCode.ValidationError, "Id can't be empty");
        }

        if (vehicleId == Guid.Empty)
        {
            return new Error(EErrorCode.ValidationError, "VehicleId can't be empty");
        }

        if (string.IsNullOrWhiteSpace(fullPath))
        {
            return new Error(EErrorCode.ValidationError, "File path can't be empty");
        }

        if (string.IsNullOrWhiteSpace(originalFileName))
        {
            return new Error(EErrorCode.ValidationError, "Name can't be empty");
        }

        if (documentType == EVehicleDocumentType.None)
        {
            return new Error(EErrorCode.ValidationError, $"Vehicle document type can't be {nameof(EVehicleDocumentType.None)}");
        }

        var document = new VehicleDocument(id, vehicleId, fullPath, originalFileName, contentType, documentType);

        document.RaiseDomainEvent(new VehicleDocumentAddedDomainEvent(document));

        return document;
    }

    public void RaiseDeletedEvent() => RaiseDomainEvent(new VehicleDocumentDeletedDomainEvent(this));
}

namespace eMechanic.Domain.Tests.Builders;

using eMechanic.Common.Result;
using eMechanic.Domain.VehicleDocument;
using eMechanic.Domain.VehicleDocument.Enums;

public class VehicleDocumentBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _vehicleId = Guid.NewGuid();
    private string _fullPath = "vehicle-documents/test-vehicle-id/test-doc-id.pdf";
    private string _originalFileName = "test-faktura.pdf";
    private string _contentType = "application/pdf";
    private EVehicleDocumentType _documentType = EVehicleDocumentType.Invoice;

    public VehicleDocumentBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public VehicleDocumentBuilder WithVehicleId(Guid vehicleId)
    {
        _vehicleId = vehicleId;
        return this;
    }

    public VehicleDocumentBuilder WithFullPath(string fullPath)
    {
        _fullPath = fullPath;
        return this;
    }

    public VehicleDocumentBuilder WithOriginalFileName(string originalFileName)
    {
        _originalFileName = originalFileName;
        return this;
    }

    public VehicleDocumentBuilder WithContentType(string contentType)
    {
        _contentType = contentType;
        return this;
    }

    public VehicleDocumentBuilder WithDocumentType(EVehicleDocumentType documentType)
    {
        _documentType = documentType;
        return this;
    }

    public Result<VehicleDocument, Error> BuildResult()
    {
        return VehicleDocument.Create(
            _id,
            _vehicleId,
            _fullPath,
            _originalFileName,
            _contentType,
            _documentType);
    }

    public VehicleDocument Build()
    {
        var result = BuildResult();
        if (result.HasError())
        {
            throw new InvalidOperationException(result.Error!.Message);
        }
        return result.Value!;
    }
}

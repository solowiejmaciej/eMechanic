namespace eMechanic.Application.Tests.Builders;

using eMechanic.Application.VehicleDocument.Features.Create;
using eMechanic.Domain.VehicleDocument.Enums;
using Microsoft.AspNetCore.Http;
using NSubstitute;

public class AddVehicleDocumentCommandBuilder
{
    private Guid _vehicleId = Guid.NewGuid();
    private IFormFile _file = CreateMockFile("test.pdf", "application/pdf", 1024);
    private EVehicleDocumentType _documentType = EVehicleDocumentType.Invoice;

    public AddVehicleDocumentCommandBuilder WithVehicleId(Guid vehicleId)
    {
        _vehicleId = vehicleId;
        return this;
    }

    public AddVehicleDocumentCommandBuilder WithDocumentType(EVehicleDocumentType documentType)
    {
        _documentType = documentType;
        return this;
    }

    public AddVehicleDocumentCommandBuilder WithFile(IFormFile file)
    {
        _file = file;
        return this;
    }

    public AddVehicleDocumentCommandBuilder WithFile(string fileName, string contentType, long length)
    {
        _file = CreateMockFile(fileName, contentType, length);
        return this;
    }

    public AddVehicleDocumentCommandBuilder WithNullFile()
    {
        _file = null!;
        return this;
    }

    public AddVehicleDocumentCommand Build() => new(_vehicleId, _file, _documentType);

    private static IFormFile CreateMockFile(string fileName, string contentType, long length)
    {
        var file = Substitute.For<IFormFile>();
        file.FileName.Returns(fileName);
        file.ContentType.Returns(contentType);
        file.Length.Returns(length);
        return file;
    }
}

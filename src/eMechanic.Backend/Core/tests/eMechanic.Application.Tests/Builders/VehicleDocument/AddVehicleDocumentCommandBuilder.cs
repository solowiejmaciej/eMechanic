
using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace eMechanic.Application.Tests.Builders.VehicleDocument;

using Application.Vehicle.Document.Features.Create;
using Domain.Vehicle.Documents.Enums;

public class AddVehicleDocumentCommandBuilder
{
    private Guid _vehicleId = Guid.NewGuid();
    private IFormFile _file = Substitute.For<IFormFile>();
    private EVehicleDocumentType _documentType = EVehicleDocumentType.Invoice;

    public AddVehicleDocumentCommandBuilder()
    {
        _file.FileName.Returns("test-faktura.pdf");
        _file.ContentType.Returns("application/pdf");
        _file.Length.Returns(1024);
        _file.OpenReadStream().Returns(new MemoryStream());
    }

    public AddVehicleDocumentCommandBuilder WithVehicleId(Guid vehicleId)
    {
        _vehicleId = vehicleId;
        return this;
    }

    public AddVehicleDocumentCommand Build()
    {
        return new AddVehicleDocumentCommand(_vehicleId, _file, _documentType);
    }
}

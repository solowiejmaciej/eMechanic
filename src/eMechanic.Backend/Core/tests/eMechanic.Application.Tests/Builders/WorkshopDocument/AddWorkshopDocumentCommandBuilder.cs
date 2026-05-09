namespace eMechanic.Application.Tests.Builders.WorkshopDocument;

using System.IO;
using Application.Workshop.Document.Features.Create;
using Domain.Workshop.Documents.Enums;
using Microsoft.AspNetCore.Http;
using NSubstitute;

public class AddWorkshopDocumentCommandBuilder
{
    private IFormFile _file = Substitute.For<IFormFile>();
    private EWorkshopDocumentType _documentType = EWorkshopDocumentType.Certificate;

    public AddWorkshopDocumentCommandBuilder()
    {
        _file.FileName.Returns("certyfikat.pdf");
        _file.ContentType.Returns("application/pdf");
        _file.Length.Returns(1024);
        _file.OpenReadStream().Returns(new MemoryStream());
    }

    public AddWorkshopDocumentCommandBuilder WithDocumentType(EWorkshopDocumentType documentType)
    {
        _documentType = documentType;
        return this;
    }

    public AddWorkshopDocumentCommand Build()
    {
        return new AddWorkshopDocumentCommand(_file, _documentType);
    }
}

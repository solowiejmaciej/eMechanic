namespace eMechanic.Domain.Tests.Builders;

using Domain.Workshop.Documents;
using Domain.Workshop.Documents.Enums;
using eMechanic.Common.Result;

public class WorkshopDocumentBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _workshopId = Guid.NewGuid();
    private Uri _fullPath = new Uri("https://storage/workshops/doc.pdf");
    private string _fileName = "test-doc.pdf";
    private EWorkshopDocumentType _documentType = EWorkshopDocumentType.Certificate;

    public WorkshopDocumentBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public WorkshopDocumentBuilder WithWorkshopId(Guid workshopId)
    {
        _workshopId = workshopId;
        return this;
    }

    public WorkshopDocumentBuilder WithFullPath(string fullPath)
    {
        _fullPath = new Uri(fullPath);
        return this;
    }

    public WorkshopDocumentBuilder WithFileName(string fileName)
    {
        _fileName = fileName;
        return this;
    }

    public WorkshopDocumentBuilder WithDocumentType(EWorkshopDocumentType documentType)
    {
        _documentType = documentType;
        return this;
    }

    public Result<WorkshopDocument, Error> BuildResult()
    {
        return WorkshopDocument.Create(
            _id,
            _workshopId,
            _fullPath,
            _fileName,
            _documentType);
    }

    public WorkshopDocument Build()
    {
        var result = BuildResult();
        if (result.HasError())
        {
            throw new InvalidOperationException(result.Error!.Message);
        }
        return result.Value!;
    }
}

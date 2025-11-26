namespace eMechanic.Application.Tests.Workshop.Document.Features.Get;

using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Storage;
using Application.Workshop.Document.Features.Get;
using Application.Workshop.Document.Repositories;
using Common.Result;
using Domain.Tests.Builders;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class GetWorkshopDocumentsQueryHandlerTests
{
    private readonly IWorkshopDocumentRepository _repository;
    private readonly IFileStorageService _fileStorage;
    private readonly GetWorkshopDocumentsQueryHandler _handler;

    public GetWorkshopDocumentsQueryHandlerTests()
    {
        _repository = Substitute.For<IWorkshopDocumentRepository>();
        _fileStorage = Substitute.For<IFileStorageService>();
        _handler = new GetWorkshopDocumentsQueryHandler(_repository, _fileStorage);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedDocuments_WhenFound()
    {
        // Arrange
        var workshopId = Guid.NewGuid();
        var doc = new WorkshopDocumentBuilder()
            .WithWorkshopId(workshopId)
            .WithFullPath("https://storage/doc.pdf")
            .Build();

        var query = new GetWorkshopDocumentsQuery(workshopId, new PaginationParameters(){PageNumber = 1, PageSize = 10});

        var pagedResult = new PaginationResult<Domain.Workshop.Documents.WorkshopDocument>(
            [doc], 1, 1, 1);

        _repository.GetByWorkshopIdAsync(workshopId, query.PaginationParameters, Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        var publicUrl = new Uri("https://public.url/doc.pdf");
        _fileStorage.GetPublicUrl(doc.FullPath).Returns(publicUrl);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(1);

        var dto = result.Value.Items.First();
        dto.Id.Should().Be(doc.Id);
        dto.PublicUrl.Should().Be(publicUrl);
        dto.FileName.Should().Be(doc.FileName);
        dto.Type.Should().Be(doc.DocumentType);
    }
}

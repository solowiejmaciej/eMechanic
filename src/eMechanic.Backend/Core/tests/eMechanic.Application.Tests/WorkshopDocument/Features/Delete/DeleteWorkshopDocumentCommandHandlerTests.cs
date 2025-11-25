namespace eMechanic.Application.Tests.WorkshopDocument.Features.Delete;

using System.Threading;
using System.Threading.Tasks;
using eMechanic.Application.Abstractions.Identity.Contexts;
using eMechanic.Application.Abstractions.Storage;
using eMechanic.Application.Workshop.Document.Features.Delete;
using eMechanic.Application.Workshop.Document.Repositories;
using eMechanic.Common.Result;
using eMechanic.Domain.Tests.Builders;
using eMechanic.Domain.Workshop.Documents;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class DeleteWorkshopDocumentCommandHandlerTests
{
    private readonly IWorkshopContext _workshopContext;
    private readonly IWorkshopDocumentRepository _repository;
    private readonly IFileStorageService _fileStorage;
    private readonly DeleteWorkshopDocumentCommandHandler _handler;

    private readonly Guid _workshopId = Guid.NewGuid();
    private readonly WorkshopDocument _document;

    public DeleteWorkshopDocumentCommandHandlerTests()
    {
        _workshopContext = Substitute.For<IWorkshopContext>();
        _repository = Substitute.For<IWorkshopDocumentRepository>();
        _fileStorage = Substitute.For<IFileStorageService>();

        _workshopContext.GetWorkshopId().Returns(_workshopId);

        _document = new WorkshopDocumentBuilder()
            .WithWorkshopId(_workshopId)
            .Build();

        _handler = new DeleteWorkshopDocumentCommandHandler(
            _workshopContext,
            _repository,
            _fileStorage);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenDocumentExistsAndBelongsToWorkshop()
    {
        // Arrange
        var command = new DeleteWorkshopDocumentCommand(_document.Id);
        _repository.GetByIdAsync(_document.Id, Arg.Any<CancellationToken>())
            .Returns(_document);

        _fileStorage.DeleteFileAsync(_document.FullPath, Arg.Any<CancellationToken>())
            .Returns(Result.Success);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await _fileStorage.Received(1).DeleteFileAsync(_document.FullPath, Arg.Any<CancellationToken>());
        await _repository.Received(1).DeleteAsync(_document, Arg.Any<CancellationToken>());
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenDocumentDoesNotExist()
    {
        // Arrange
        var command = new DeleteWorkshopDocumentCommand(Guid.NewGuid());
        _repository.GetByIdAsync(command.DocumentId, Arg.Any<CancellationToken>())
            .Returns((WorkshopDocument?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.NotFoundError);

        await _fileStorage.DidNotReceive().DeleteFileAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenDocumentBelongsToOtherWorkshop()
    {
        // Arrange
        var otherWorkshopDoc = new WorkshopDocumentBuilder()
            .WithWorkshopId(Guid.NewGuid())
            .Build();

        var command = new DeleteWorkshopDocumentCommand(otherWorkshopDoc.Id);

        _repository.GetByIdAsync(otherWorkshopDoc.Id, Arg.Any<CancellationToken>())
            .Returns(otherWorkshopDoc);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.NotFoundError);

        await _fileStorage.DidNotReceive().DeleteFileAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _repository.DidNotReceive().DeleteAsync(Arg.Any<WorkshopDocument>(), Arg.Any<CancellationToken>());
    }
}

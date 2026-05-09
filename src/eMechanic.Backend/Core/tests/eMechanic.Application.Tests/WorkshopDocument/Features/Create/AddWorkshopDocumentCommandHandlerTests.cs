namespace eMechanic.Application.Tests.WorkshopDocument.Features.Create;

using System.Threading;
using System.Threading.Tasks;
using eMechanic.Application.Abstractions.Identity.Contexts;
using eMechanic.Application.Abstractions.Storage;
using eMechanic.Application.Tests.Builders.WorkshopDocument;
using eMechanic.Application.Workshop.Document.Features.Create;
using eMechanic.Application.Workshop.Document.Repositories;
using eMechanic.Common.Result;
using eMechanic.Domain.Workshop.Documents;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class AddWorkshopDocumentCommandHandlerTests
{
    private readonly IWorkshopContext _workshopContext;
    private readonly IWorkshopDocumentPathBuilder _pathBuilder;
    private readonly IFileStorageService _fileStorage;
    private readonly IWorkshopDocumentRepository _repository;
    private readonly AddWorkshopDocumentCommandHandler _handler;

    private readonly AddWorkshopDocumentCommand _command;
    private readonly Guid _workshopId = Guid.NewGuid();
    private readonly Uri _generatedPath = new Uri("https://azure.blob/workshops/doc.pdf");
    private readonly Uri _publicUrl = new Uri("https://public.url/doc.pdf");

    public AddWorkshopDocumentCommandHandlerTests()
    {
        _workshopContext = Substitute.For<IWorkshopContext>();
        _pathBuilder = Substitute.For<IWorkshopDocumentPathBuilder>();
        _fileStorage = Substitute.For<IFileStorageService>();
        _repository = Substitute.For<IWorkshopDocumentRepository>();

        _command = new AddWorkshopDocumentCommandBuilder().Build();

        _handler = new AddWorkshopDocumentCommandHandler(
            _workshopContext,
            _pathBuilder,
            _fileStorage,
            _repository);

        _workshopContext.GetWorkshopId().Returns(_workshopId);

        _pathBuilder.BuildNewDocumentPath(_workshopId, Arg.Any<Guid>(), Arg.Any<string>())
            .Returns(_generatedPath);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenUploadAndSaveAreSuccessful()
    {
        // Arrange
        _fileStorage.UploadFileAsync(_generatedPath.ToString(), _command.File, Arg.Any<CancellationToken>())
            .Returns(Result.Success);

        _fileStorage.GetPublicUrl(_generatedPath.ToString())
            .Returns(_publicUrl);

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(_publicUrl);

        await _fileStorage.Received(1).UploadFileAsync(_generatedPath.ToString(), _command.File, Arg.Any<CancellationToken>());

        await _repository.Received(1).AddAsync(Arg.Is<WorkshopDocument>(d =>
            d.WorkshopId == _workshopId &&
            d.FullPath == _generatedPath.ToString() &&
            d.FileName == _command.File.FileName
        ), Arg.Any<CancellationToken>());

        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenFileUploadFails()
    {
        // Arrange
        var error = new Error(EErrorCode.InternalServerError, "Storage failed");
        _fileStorage.UploadFileAsync(_generatedPath.ToString(), _command.File, Arg.Any<CancellationToken>())
            .Returns(error);

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error.Should().Be(error);

        await _repository.DidNotReceive().AddAsync(Arg.Any<WorkshopDocument>(), Arg.Any<CancellationToken>());
        await _repository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

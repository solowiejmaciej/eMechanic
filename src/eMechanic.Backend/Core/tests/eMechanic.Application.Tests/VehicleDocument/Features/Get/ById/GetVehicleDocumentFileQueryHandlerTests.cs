namespace eMechanic.Application.Tests.VehicleDocument.Features.Get.ById;

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eMechanic.Application.Abstractions.Storage;
using eMechanic.Application.Storage.Dtos;
using eMechanic.Application.Tests.Builders;
using eMechanic.Application.Vehicle.Services;
using eMechanic.Application.VehicleDocument.Features.Get.ById;
using eMechanic.Application.VehicleDocument.Repositories;
using eMechanic.Common.Result;
using eMechanic.Domain.Tests.Builders;
using eMechanic.Domain.Vehicle;
using eMechanic.Domain.VehicleDocument;
using FluentAssertions;
using NSubstitute;

public class GetVehicleDocumentFileQueryHandlerTests
{
    private readonly IVehicleOwnershipService _ownershipService;
    private readonly IVehicleDocumentRepository _documentRepository;
    private readonly IFileStorageService _fileStorage;
    private readonly GetVehicleDocumentFileQueryHandler _handler;

    private readonly Guid _vehicleId = Guid.NewGuid();
    private readonly Guid _documentId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Vehicle _vehicle;
    private readonly VehicleDocument _document;
    private readonly GetVehicleDocumentFileQuery _query;
    private readonly FileDownloadResult _fileDownloadResult;

    public GetVehicleDocumentFileQueryHandlerTests()
    {
        _ownershipService = Substitute.For<IVehicleOwnershipService>();
        _documentRepository = Substitute.For<IVehicleDocumentRepository>();
        _fileStorage = Substitute.For<IFileStorageService>();

        _handler = new GetVehicleDocumentFileQueryHandler(
            _ownershipService, _documentRepository, _fileStorage, Substitute.For<Microsoft.Extensions.Logging.ILogger<GetVehicleDocumentFileQueryHandler>>());

        _vehicle = new VehicleBuilder().WithOwnerId(_userId).Build();
        _document = new VehicleDocumentBuilder()
            .WithId(_documentId)
            .WithVehicleId(_vehicleId)
            .Build();

        _query = new GetVehicleDocumentFileQueryBuilder()
            .WithVehicleId(_vehicleId)
            .WithDocumentId(_documentId)
            .Build();

        _fileDownloadResult = new FileDownloadResult(
            new MemoryStream(Encoding.UTF8.GetBytes("test file")), "application/pdf", "test-faktura.pdf");

        _ownershipService.GetAndVerifyOwnershipAsync(_vehicleId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Result<Vehicle, Error>>(_vehicle));

        _documentRepository.GetByIdAsync(_documentId, Arg.Any<CancellationToken>())
            .Returns(_document);
        _fileStorage.GetFileAsync(_document.FullPath, Arg.Any<CancellationToken>(), _document.OriginalFileName)
            .Returns(_fileDownloadResult);
    }

    [Fact]
    public async Task Handle_Should_ReturnFileDownloadResult_WhenCommandIsValid()
    {
        // Act
        var result = await _handler.Handle(_query, CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().Be(_fileDownloadResult);
        result.Value.FileName.Should().Be("test-faktura.pdf");
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenOwnershipFails()
    {
        // Arrange
        var error = new Error(EErrorCode.NotFoundError, "Not owner");
        _ownershipService.GetAndVerifyOwnershipAsync(_vehicleId, Arg.Any<CancellationToken>())
            .Returns(error);

        // Act
        var result = await _handler.Handle(_query, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error.Should().Be(error);
        await _documentRepository.DidNotReceiveWithAnyArgs().GetByIdAsync(default, default);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFoundError_WhenDocumentNotFound()
    {
        // Arrange
        _documentRepository.GetByIdAsync(_documentId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<VehicleDocument?>(null));

        // Act
        var result = await _handler.Handle(_query, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.NotFoundError);
    }

    [Fact]
    public async Task Handle_Should_ThrowUnauthorizedAccessException_WhenDocumentBelongsToAnotherVehicle()
    {
        // Arrange
        var otherVehicleId = Guid.NewGuid();
        var otherDocument = new VehicleDocumentBuilder()
            .WithId(_documentId)
            .WithVehicleId(otherVehicleId)
            .Build();

        _documentRepository.GetByIdAsync(_documentId, Arg.Any<CancellationToken>())
            .Returns(otherDocument);

        // Act
        Func<Task> act = () => _handler.Handle(_query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        await _fileStorage.DidNotReceiveWithAnyArgs().GetFileAsync(default!, default, default!);
    }
}

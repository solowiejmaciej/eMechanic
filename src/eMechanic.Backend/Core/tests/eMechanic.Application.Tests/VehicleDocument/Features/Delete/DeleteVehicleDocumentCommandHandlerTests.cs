namespace eMechanic.Application.Tests.VehicleDocument.Features.Delete;

using System;
using System.Threading;
using System.Threading.Tasks;
using eMechanic.Application.Abstractions.Storage;
using eMechanic.Application.Vehicle.Services;
using eMechanic.Application.VehicleDocument.Features.Delete;
using eMechanic.Application.VehicleDocument.Repositories;
using eMechanic.Common.Result;
using eMechanic.Domain.Tests.Builders;
using eMechanic.Domain.Vehicle;
using eMechanic.Domain.VehicleDocument;
using FluentAssertions;
using NSubstitute;

public class DeleteVehicleDocumentCommandHandlerTests
{
    private readonly IVehicleOwnershipService _ownershipService;
    private readonly IVehicleDocumentRepository _documentRepository;
    private readonly IFileStorageService _fileStorage;
    private readonly DeleteVehicleDocumentCommandHandler _handler;

    private readonly Guid _vehicleId = Guid.NewGuid();
    private readonly Guid _documentId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();
    private readonly VehicleDocument _document;
    private readonly DeleteVehicleDocumentCommand _command;

    public DeleteVehicleDocumentCommandHandlerTests()
    {
        _ownershipService = Substitute.For<IVehicleOwnershipService>();
        _documentRepository = Substitute.For<IVehicleDocumentRepository>();
        _fileStorage = Substitute.For<IFileStorageService>();

        var vehicle = new VehicleBuilder().WithOwnerId(_userId).Build();
        _document = new VehicleDocumentBuilder()
            .WithId(_documentId)
            .WithVehicleId(_vehicleId)
            .Build();

        _command = new DeleteVehicleDocumentCommand(_vehicleId, _documentId);

        _handler = new DeleteVehicleDocumentCommandHandler(
            _ownershipService, _documentRepository, _fileStorage, Substitute.For<Microsoft.Extensions.Logging.ILogger<DeleteVehicleDocumentCommandHandler>>());

        _ownershipService.GetAndVerifyOwnershipAsync(_vehicleId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Result<Vehicle, Error>>(vehicle));

        _documentRepository.GetByIdAsync(_documentId, Arg.Any<CancellationToken>())
            .Returns(_document);
        _fileStorage.DeleteFileAsync(_document.FullPath, Arg.Any<CancellationToken>())
            .Returns(Result.Success);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenCommandIsValid()
    {
        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();

        _documentRepository.Received(1).DeleteAsync(_document, Arg.Any<CancellationToken>());
        await _documentRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _fileStorage.Received(1).DeleteFileAsync(_document.FullPath, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenOwnershipFails()
    {
        // Arrange
        var error = new Error(EErrorCode.NotFoundError, "Not owner");
        _ownershipService.GetAndVerifyOwnershipAsync(_vehicleId, Arg.Any<CancellationToken>())
            .Returns(error);

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error.Should().Be(error);
        _documentRepository.DidNotReceiveWithAnyArgs().DeleteAsync(default!, default);
        await _fileStorage.DidNotReceiveWithAnyArgs().DeleteFileAsync(default!, default);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFoundError_WhenDocumentNotFound()
    {
        // Arrange
        _documentRepository.GetByIdAsync(_documentId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<VehicleDocument?>(null));

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.NotFoundError);
        _documentRepository.DidNotReceiveWithAnyArgs().DeleteAsync(default!, default);
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
        Func<Task> act = () => _handler.Handle(_command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        _documentRepository.DidNotReceiveWithAnyArgs().DeleteAsync(default!, default);
        await _fileStorage.DidNotReceiveWithAnyArgs().DeleteFileAsync(default!, default);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenFileStorageDeleteFails()
    {
        // Arrange
        var storageError = new Error(EErrorCode.InternalServerError, "Storage failed");
        _fileStorage.DeleteFileAsync(_document.FullPath, Arg.Any<CancellationToken>())
            .Returns(storageError);

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error.Should().Be(storageError);

        _documentRepository.Received(1).DeleteAsync(_document, Arg.Any<CancellationToken>());
        await _documentRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

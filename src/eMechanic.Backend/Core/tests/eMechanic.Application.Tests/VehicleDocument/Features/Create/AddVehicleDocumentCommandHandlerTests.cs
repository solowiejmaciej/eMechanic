namespace eMechanic.Application.Tests.VehicleDocument.Features.Create;

using System.Threading;
using System.Threading.Tasks;
using eMechanic.Application.Abstractions.Storage;
using eMechanic.Application.Tests.Builders;
using eMechanic.Application.Vehicle.Services;
using eMechanic.Application.VehicleDocument.Features.Create;
using eMechanic.Application.VehicleDocument.Repositories;
using eMechanic.Common.Result;
using eMechanic.Domain.Tests.Builders;
using eMechanic.Domain.Vehicle;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

public class AddVehicleDocumentCommandHandlerTests
{
    private readonly IVehicleOwnershipService _ownershipService;
    private readonly IVehicleDocumentRepository _documentRepository;
    private readonly IFileStorageService _fileStorage;
    private readonly AddVehicleDocumentCommandHandler _handler;

    private readonly Guid _vehicleId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();
    private readonly IFormFile _mockFile;
    private readonly AddVehicleDocumentCommand _command;
    private const string EXPECTED_PATH = "vehicle-documents/path/test.pdf";

    public AddVehicleDocumentCommandHandlerTests()
    {
        _ownershipService = Substitute.For<IVehicleOwnershipService>();
        _documentRepository = Substitute.For<IVehicleDocumentRepository>();
        var pathBuilder = Substitute.For<IVehicleDocumentPathBuilder>();
        _fileStorage = Substitute.For<IFileStorageService>();

        var vehicle = new VehicleBuilder().WithOwnerId(_userId).Build();

        var builder = new AddVehicleDocumentCommandBuilder().WithVehicleId(_vehicleId);
        _command = builder.Build();
        _mockFile = _command.File;

        _handler = new AddVehicleDocumentCommandHandler(
            _ownershipService, _documentRepository, pathBuilder, _fileStorage);

        _ownershipService.GetAndVerifyOwnershipAsync(_vehicleId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Result<Vehicle, Error>>(vehicle));

        pathBuilder.BuildNewDocumentPath(Arg.Is(_vehicleId), Arg.Any<Guid>(), Arg.Is(_mockFile.FileName))
            .Returns(EXPECTED_PATH);

        _fileStorage.UploadFileAsync(EXPECTED_PATH, _mockFile, Arg.Any<CancellationToken>())
            .Returns(Result.Success);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenCommandIsValid()
    {
        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeEmpty();

        await _fileStorage.Received(1).UploadFileAsync(EXPECTED_PATH, _mockFile, Arg.Any<CancellationToken>());
        await _documentRepository.Received(1).AddAsync(Arg.Is<Domain.VehicleDocument.VehicleDocument>(
            d => d.VehicleId == _vehicleId && d.FullPath == EXPECTED_PATH
        ), Arg.Any<CancellationToken>());
        await _documentRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenOwnershipServiceFails()
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
        await _fileStorage.DidNotReceiveWithAnyArgs().UploadFileAsync(default!, default!, default);
        await _documentRepository.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenFileStorageUploadFails()
    {
        // Arrange
        var storageError = new Error(EErrorCode.InternalServerError, "Storage failed");
        _fileStorage.UploadFileAsync(EXPECTED_PATH, _mockFile, Arg.Any<CancellationToken>())
            .Returns(storageError);

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error.Should().Be(storageError);
        await _documentRepository.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
    }

    [Fact]
    public async Task Handle_Should_ReturnErrorAndRollbackStorage_WhenRepositorySaveFails()
    {
        // Arrange
        _documentRepository.SaveChangesAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("DB connection lost"));

        // Act
        Func<Task> act = () => _handler.Handle(_command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        await _fileStorage.Received(1).DeleteFileAsync(EXPECTED_PATH, Arg.Any<CancellationToken>());
    }
}

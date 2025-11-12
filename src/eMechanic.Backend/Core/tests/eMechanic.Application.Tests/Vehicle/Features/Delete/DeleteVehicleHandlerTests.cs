namespace eMechanic.Application.Tests.Vehicle.Features.Delete;

using Application.Tests.Builders;
using Application.Vehicle.Repostories;
using Domain.Tests.Builders;
using eMechanic.Application.Abstractions.Identity.Contexts;
using eMechanic.Application.Vehicle.Features.Delete;
using eMechanic.Common.Result;
using eMechanic.Domain.Vehicle;
using eMechanic.Domain.Vehicle.Enums;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

public class DeleteVehicleHandlerTests
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUserContext _userContext;
    private readonly DeleteVehicleHandler _handler;

    private readonly Guid _currentUserId = Guid.NewGuid();
    private readonly Guid _vehicleId = Guid.NewGuid();
    private readonly Vehicle _existingVehicle;

    public DeleteVehicleHandlerTests()
    {
        _vehicleRepository = Substitute.For<IVehicleRepository>();
        _userContext = Substitute.For<IUserContext>();
        _userContext.GetUserId().Returns(_currentUserId);
        _userContext.IsAuthenticated.Returns(true);

        var creationResult = new VehicleBuilder().WithOwnerId(_currentUserId).BuildResult();
        creationResult.HasError().Should().BeFalse();
        _existingVehicle = creationResult.Value!;
        typeof(Vehicle).GetProperty("Id")!.SetValue(_existingVehicle, _vehicleId);


        _handler = new DeleteVehicleHandler(_vehicleRepository, _userContext);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenVehicleExistsForUserAndIsDeleted()
    {
        // Arrange
        var command = new DeleteVehicleCommandBuilder().WithId(_vehicleId).Build();

        _vehicleRepository.GetForUserById(_vehicleId, _currentUserId, Arg.Any<CancellationToken>())
             .Returns(Task.FromResult<Vehicle?>(_existingVehicle));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();
        _vehicleRepository.Received(1).DeleteAsync(Arg.Is(_existingVehicle), Arg.Any<CancellationToken>());
        await _vehicleRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFoundError_WhenVehicleDoesNotExistForUser()
    {
        // Arrange
        var command = new DeleteVehicleCommandBuilder().WithId(_vehicleId).Build();

        _vehicleRepository.GetForUserById(_vehicleId, _currentUserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Vehicle?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.NotFoundError);
        _vehicleRepository.DidNotReceiveWithAnyArgs().DeleteAsync(default!, default);
        await _vehicleRepository.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_Should_ThrowUnauthorizedAccessException_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var command = new DeleteVehicleCommandBuilder().WithId(_vehicleId).Build();
        _userContext.GetUserId().ThrowsForAnyArgs<UnauthorizedAccessException>();

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, CancellationToken.None));
        await _vehicleRepository.DidNotReceiveWithAnyArgs().GetForUserById(default, default, default);
    }
}

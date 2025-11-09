namespace eMechanic.Application.Tests.Vehicle.Delete;

using Application.Abstractions.Identity.Contexts;
using Application.Abstractions.Vehicle;
using Application.Vehicle.Features.Delete;
using Common.Result;
using Domain.Vehicle;
using Domain.Vehicle.Enums;
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

        var creationResult = Vehicle.Create(
            _currentUserId,
            "V1N123456789ABCDE",
            "ToDelete",
            "ToDelete",
            "2021",
            1.0m,
            200,
            EMileageUnit.Miles,
            "PZ1W924",
            124,
            EFuelType.LPG,
            EBodyType.Van,
            EVehicleType.Passenger);
        creationResult.HasError().Should().BeFalse();
        _existingVehicle = creationResult.Value!;
        typeof(Vehicle).GetProperty("Id")!.SetValue(_existingVehicle, _vehicleId);


        _handler = new DeleteVehicleHandler(_vehicleRepository, _userContext);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenVehicleExistsForUserAndIsDeleted()
    {
        // Arrange
        var command = new DeleteVehicleCommand(_vehicleId);

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
        var command = new DeleteVehicleCommand(_vehicleId);

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
        var command = new DeleteVehicleCommand(_vehicleId);
        _userContext.GetUserId().ThrowsForAnyArgs<UnauthorizedAccessException>();

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, CancellationToken.None));
        await _vehicleRepository.DidNotReceiveWithAnyArgs().GetForUserById(default, default, default);
    }
}

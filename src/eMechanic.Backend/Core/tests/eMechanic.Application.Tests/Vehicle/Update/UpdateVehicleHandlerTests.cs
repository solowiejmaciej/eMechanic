namespace eMechanic.Application.Tests.Vehicle.Update;

using Application.Abstractions.Identity.Contexts;
using Application.Abstractions.Vehicle;
using Application.Vehicle.Update;
using Common.Result;
using Domain.Vehicle;
using Domain.Vehicle.Enums;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

public class UpdateVehicleHandlerTests
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUserContext _userContext;
    private readonly UpdateVehicleHandler _handler;

    private readonly Guid _currentUserId = Guid.NewGuid();
    private readonly Guid _vehicleId = Guid.NewGuid();
    private readonly Vehicle _existingVehicle;

    public UpdateVehicleHandlerTests()
    {
        _vehicleRepository = Substitute.For<IVehicleRepository>();
        _userContext = Substitute.For<IUserContext>();
        _userContext.UserId.Returns(_currentUserId);
        _userContext.IsAuthenticated.Returns(true);

        var creationResult = Vehicle.Create(
            _currentUserId, "V1N123456789ABCDE", "Old Manufacturer", "Old Model", "2020",
            1.5m, EFuelType.Diesel, EBodyType.Kombi, EVehicleType.Passenger);
        creationResult.HasError().Should().BeFalse();
        _existingVehicle = creationResult.Value!;
        typeof(Vehicle).GetProperty("Id")!.SetValue(_existingVehicle, _vehicleId);


        _handler = new UpdateVehicleHandler(_vehicleRepository, _userContext);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenCommandIsValidAndVehicleExistsForUser()
    {
        // Arrange
        var command = new UpdateVehicleCommand(
            _vehicleId, "V1N123456789ABCDE", "New Manufacturer", "New Model", "2024",
            2.0m, EFuelType.Electric, EBodyType.SUV, EVehicleType.Passenger);

        _vehicleRepository.GetForUserById(_vehicleId, _currentUserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Vehicle?>(_existingVehicle));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();
        _vehicleRepository.Received(1).UpdateAsync(Arg.Is<Vehicle>(v =>
            v.Id == _vehicleId &&
            v.Manufacturer.Value == command.Manufacturer &&
            v.Model.Value == command.Model &&
            v.Vin.Value == command.Vin &&
            v.ProductionYear.Value == command.ProductionYear &&
            v.EngineCapacity!.Value == command.EngineCapacity &&
            v.FuelType == command.FuelType &&
            v.BodyType == command.BodyType &&
            v.VehicleType == command.VehicleType
        ), Arg.Any<CancellationToken>());
        await _vehicleRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFoundError_WhenVehicleDoesNotExistForUser()
    {
        // Arrange
        var command = new UpdateVehicleCommand(
             _vehicleId, "V1N123456789ABCDE", "New Manufacturer", "New Model", "2024",
             2.0m, EFuelType.Electric, EBodyType.SUV, EVehicleType.Passenger);

        _vehicleRepository.GetForUserById(_vehicleId, _currentUserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Vehicle?>(null)); // Vehicle not found

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.NotFoundError);
        _vehicleRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
        await _vehicleRepository.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

     [Fact]
    public async Task Handle_Should_ReturnValidationError_WhenDomainUpdateFails()
    {
        // Arrange
         var command = new UpdateVehicleCommand(
             _vehicleId, "dasdasdasdas", "New Manufacturer", "New Model", "2024",
             2.0m, EFuelType.Electric, EBodyType.SUV, EVehicleType.Passenger);

        _vehicleRepository.GetForUserById(_vehicleId, _currentUserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Vehicle?>(_existingVehicle));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        _vehicleRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
        await _vehicleRepository.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

     [Fact]
    public async Task Handle_Should_ThrowUnauthorizedAccessException_WhenUserIsNotAuthenticated()
    {
        // Arrange
         var command = new UpdateVehicleCommand(
            _vehicleId, "V1N123456789ABCDE", "Test Manufacturer", "Test Model", "2023",
            1.6m, EFuelType.Gasoline, EBodyType.Sedan, EVehicleType.Passenger);
         _userContext.UserId.ThrowsForAnyArgs<UnauthorizedAccessException>();
        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, CancellationToken.None));
        await _vehicleRepository.DidNotReceiveWithAnyArgs().GetForUserById(default, default, default);
    }
}

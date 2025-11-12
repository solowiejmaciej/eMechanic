namespace eMechanic.Application.Tests.Vehicle.Features.Update;

using Application.Tests.Builders;
using Application.Vehicle.Repostories;
using Application.Vehicle.Services;
using Domain.Tests.Builders;
using eMechanic.Application.Vehicle.Features.Update;
using eMechanic.Common.Result;
using eMechanic.Domain.Vehicle;
using eMechanic.Domain.Vehicle.Enums;
using FluentAssertions;
using NSubstitute;

public class UpdateVehicleHandlerTests
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IVehicleOwnershipService _ownershipService;
    private readonly UpdateVehicleHandler _handler;

    private readonly Guid _currentUserId = Guid.NewGuid();
    private readonly Guid _vehicleId = Guid.NewGuid();
    private readonly Vehicle _existingVehicle;

    public UpdateVehicleHandlerTests()
    {
        _vehicleRepository = Substitute.For<IVehicleRepository>();
        _ownershipService = Substitute.For<IVehicleOwnershipService>();
        var creationResult = new VehicleBuilder()
            .WithOwnerId(_currentUserId)
            .WithManufacturer("Old Manufacturer")
            .WithModel("Old Model")
            .WithProductionYear("2020")
            .WithEngineCapacity(1.5m)
            .WithMileage(200, EMileageUnit.Miles)
            .WithLicensePlate("PZ1W924")
            .WithHorsePower(124)
            .WithFuelType(EFuelType.Diesel)
            .WithBodyType(EBodyType.Kombi)
            .WithVehicleType(EVehicleType.Passenger)
            .BuildResult();

        creationResult.HasError().Should().BeFalse();
        _existingVehicle = creationResult.Value!;
        typeof(Vehicle).GetProperty("Id")!.SetValue(_existingVehicle, _vehicleId);

        _handler = new UpdateVehicleHandler(_vehicleRepository, _ownershipService);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenCommandIsValidAndVehicleExistsForUser()
    {
        // Arrange
        var command = new UpdateVehicleCommandBuilder()
            .WithId(_vehicleId)
            .Build();

        _ownershipService.GetAndVerifyOwnershipAsync(_vehicleId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Result<Vehicle, Error>>(_existingVehicle));

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
    public async Task Handle_Should_ReturnNotFoundError_WhenOwnershipServiceFails()
    {
        // Arrange
        var command = new UpdateVehicleCommandBuilder().WithId(_vehicleId).Build();

        var notFoundError = new Error(EErrorCode.NotFoundError, "Vehicle not found.");
        _ownershipService.GetAndVerifyOwnershipAsync(_vehicleId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Result<Vehicle, Error>>(notFoundError));

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
        var command = new UpdateVehicleCommandBuilder().WithId(_vehicleId).WithVin("zly-vin").Build();

        _ownershipService.GetAndVerifyOwnershipAsync(_vehicleId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Result<Vehicle, Error>>(_existingVehicle));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);

        _vehicleRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
        await _vehicleRepository.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_Should_ReturnUnauthorizedError_WhenOwnershipServiceReturnsUnauthorized()
    {
        // Arrange
        var command = new UpdateVehicleCommandBuilder().WithId(_vehicleId).Build();

        var unauthorizedError = new Error(EErrorCode.UnauthorizedError, "User is not authenticated.");
        _ownershipService.GetAndVerifyOwnershipAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Result<Vehicle, Error>>(unauthorizedError));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.UnauthorizedError);
        await _vehicleRepository.DidNotReceiveWithAnyArgs().GetForUserById(default, default, default);
    }
}

namespace eMechanic.Application.Tests.Repair.PaymentStrategies;

using Application.Repair.Repositories;
using Application.Vehicle.Vehicle.Services;
using Common.Result;
using Domain.Repair.Enums;
using Domain.Tests.Builders;
using FluentAssertions;
using NSubstitute;
using eMechanic.Application.Repair.PaymentStrategies;

public class RepairPaymentInitializationStrategyTests
{
    private readonly IRepairRepository _repairRepository = Substitute.For<IRepairRepository>();
    private readonly IVehicleOwnershipService _vehicleOwnershipService = Substitute.For<IVehicleOwnershipService>();
    private readonly RepairPaymentInitializationStrategy _strategy;

    public RepairPaymentInitializationStrategyTests()
    {
        _strategy = new RepairPaymentInitializationStrategy(_repairRepository, _vehicleOwnershipService);
    }

    [Fact]
    public async Task BuildPayableItem_Should_ReturnPayableItem_WhenRepairIsCompleted()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var vehicle = new VehicleBuilder().WithOwnerId(ownerId).Build();
        var repair = new RepairBuilder().WithVehicleId(vehicle.Id).WithStatus(ERepairStatus.Completed).Build();

        _repairRepository.GetByIdAsync(repair.Id, Arg.Any<CancellationToken>()).Returns(repair);
        _vehicleOwnershipService.GetAndVerifyOwnershipAsync(repair.VehicleId, Arg.Any<CancellationToken>())
            .Returns(vehicle);

        // Act
        var result = await _strategy.BuildPayableItemAsync(repair.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.ReferenceId.Should().Be(repair.Id);
        result.Value.PayerId.Should().Be(ownerId);
        result.Value.Amount.Should().Be(repair.FinalCost);
    }

    [Fact]
    public async Task BuildPayableItem_Should_ReturnNotFound_WhenRepairDoesNotExist()
    {
        // Arrange
        _repairRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Domain.Repair.Repair?)null);

        // Act
        var result = await _strategy.BuildPayableItemAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(EErrorCode.NotFoundError);
    }

    [Fact]
    public async Task BuildPayableItem_Should_ReturnValidationError_WhenRepairNotCompleted()
    {
        // Arrange
        var repair = new RepairBuilder().WithStatus(ERepairStatus.InProgress).Build();
        _repairRepository.GetByIdAsync(repair.Id, Arg.Any<CancellationToken>()).Returns(repair);

        // Act
        var result = await _strategy.BuildPayableItemAsync(repair.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        await _vehicleOwnershipService.DidNotReceive()
            .GetAndVerifyOwnershipAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task BuildPayableItem_Should_PropagateOwnershipError_WhenOwnershipFails()
    {
        // Arrange
        var repair = new RepairBuilder().WithStatus(ERepairStatus.Completed).Build();
        _repairRepository.GetByIdAsync(repair.Id, Arg.Any<CancellationToken>()).Returns(repair);
        _vehicleOwnershipService.GetAndVerifyOwnershipAsync(repair.VehicleId, Arg.Any<CancellationToken>())
            .Returns(new Error(EErrorCode.UnauthorizedError, "Not the vehicle owner."));

        // Act
        var result = await _strategy.BuildPayableItemAsync(repair.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(EErrorCode.UnauthorizedError);
    }
}


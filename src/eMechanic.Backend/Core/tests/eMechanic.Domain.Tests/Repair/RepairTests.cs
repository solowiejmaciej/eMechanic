namespace eMechanic.Domain.Tests.Repair;

using Builders;
using Common.Result;
using Domain.Repair.DomainEvents;
using Domain.Repair.Enums;
using Domain.Shared.ValueObjects;
using FluentAssertions;

public class RepairTests
{
    [Fact]
    public void Create_Should_CreateScheduledRepairAndRaiseCreatedEvent_WhenDataIsValid()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var workshopId = Guid.NewGuid();
        var estimatedCost = Money.Create(500m, "PLN").Value!;

        // Act
        var result = Domain.Repair.Repair.Create(vehicleId, workshopId, estimatedCost, Guid.NewGuid());

        // Assert
        result.HasError().Should().BeFalse();
        var repair = result.Value!;
        repair.VehicleId.Should().Be(vehicleId);
        repair.WorkshopId.Should().Be(workshopId);
        repair.EstimatedCost.Should().Be(estimatedCost);
        repair.FinalCost.Should().BeNull();
        repair.Status.Should().Be(ERepairStatus.Scheduled);
        repair.GetDomainEvents().Should().ContainSingle(e => e is RepairCreatedDomainEvent);
    }

    [Fact]
    public void StartRepair_Should_ChangeStatusAndRaiseEvent_WhenStatusIsScheduled()
    {
        // Arrange
        var repair = new RepairBuilder().Build();
        repair.ClearDomainEvents();

        // Act
        var result = repair.StartRepair();

        // Assert
        result.HasError().Should().BeFalse();
        repair.Status.Should().Be(ERepairStatus.InProgress);
        repair.GetDomainEvents().Should().ContainSingle(e => e is RepairStartedDomainEvent);
    }

    [Fact]
    public void CompleteRepair_Should_ChangeStatusAndFinalCostAndRaiseEvent_WhenStatusIsInProgress()
    {
        // Arrange
        var repair = new RepairBuilder().WithStatus(ERepairStatus.InProgress).Build();
        var finalCost = Money.Create(650m, "PLN").Value!;
        repair.ClearDomainEvents();

        // Act
        var result = repair.CompleteRepair(finalCost);

        // Assert
        result.HasError().Should().BeFalse();
        repair.Status.Should().Be(ERepairStatus.Completed);
        repair.FinalCost.Should().Be(finalCost);
        repair.GetDomainEvents().Should().ContainSingle(e => e is RepairCompletedDomainEvent);
    }

    [Fact]
    public void Pay_Should_ReturnErrorAndNotChangeState_WhenStatusIsScheduled()
    {
        // Arrange
        var repair = new RepairBuilder().WithStatus(ERepairStatus.Scheduled).Build();
        var originalStatus = repair.Status;
        repair.ClearDomainEvents();

        // Act
        var result = repair.Pay();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        repair.Status.Should().Be(originalStatus);
        repair.GetDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void Pay_Should_ChangeStatusAndRaiseEvent_WhenStatusIsCompleted()
    {
        // Arrange
        var repair = new RepairBuilder().WithStatus(ERepairStatus.Completed).Build();
        repair.ClearDomainEvents();

        // Act
        var result = repair.Pay();

        // Assert
        result.HasError().Should().BeFalse();
        repair.Status.Should().Be(ERepairStatus.Paid);
        repair.GetDomainEvents().Should().ContainSingle(e => e is RepairPaidDomainEvent);
    }
}


namespace eMechanic.Domain.Tests.RepairRequest;

using System;
using Builders;
using Common.Result;
using Domain.RepairRequest.Enums;
using Domain.RepairRequest.DomainEvents;
using FluentAssertions;
using Xunit;

public class RepairRequestTests
{
    private readonly Guid _vehicleId = Guid.NewGuid();
    private readonly Guid _workshopId = Guid.NewGuid();

    [Fact]
    public void Create_Should_ReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var builder = new RepairRequestBuilder()
            .WithVehicleId(_vehicleId)
            .WithWorkshopId(_workshopId);

        // Act
        var result = builder.BuildResult();

        // Assert
        result.HasError().Should().BeFalse();
        var request = result.Value!;

        request.Id.Should().NotBeEmpty();
        request.VehicleId.Should().Be(_vehicleId);
        request.WorkshopId.Should().Be(_workshopId);
        request.Status.Should().Be(ERepairRequestStatus.Pending);
        request.Description.Value.Should().NotBeNullOrWhiteSpace();

        // Domain Events
        request.GetDomainEvents().Should().ContainSingle(e => e is RepairRequestCreatedDomainEvent);
    }

    [Fact]
    public void Create_Should_ReturnError_WhenVehicleIdIsEmpty()
    {
        // Act
        var result = new RepairRequestBuilder().WithVehicleId(Guid.Empty).BuildResult();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("VehicleId");
    }

    [Fact]
    public void ProvideEstimation_Should_UpdateStateToEstimated_WhenRequestIsPending()
    {
        // Arrange
        var request = new RepairRequestBuilder().Build();
        request.ClearDomainEvents();
        var diagnosis = "Brake pads worn out.";
        var cost = 450.00m;

        // Act
        var result = request.ProvideEstimation(diagnosis, cost);

        // Assert
        result.HasError().Should().BeFalse();
        request.Status.Should().Be(ERepairRequestStatus.Estimated);
        request.Diagnosis!.Value.Should().Be(diagnosis);
        request.EstimatedCost!.Amount.Should().Be(cost);

        request.GetDomainEvents().Should().ContainSingle(e => e is RepairRequestEstimatedDomainEvent);
    }

    [Fact]
    public void ProvideEstimation_Should_ReturnError_WhenCostIsNegative()
    {
        // Arrange
        var request = new RepairRequestBuilder().Build();

        // Act
        var result = request.ProvideEstimation("Diagnosis", -100m);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Message.Should().Contain("negative");
    }

    [Fact]
    public void ProvideEstimation_Should_ReturnError_WhenStatusIsNotPending()
    {
        // Arrange
        var request = new RepairRequestBuilder().Build();
        request.ProvideEstimation("Initial diagnosis", 100m);

        // Act
        var result = request.ProvideEstimation("New diagnosis", 200m);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("Pending");
    }

    [Fact]
    public void AcceptEstimation_Should_UpdateStateToAccepted_WhenStatusIsEstimated()
    {
        // Arrange
        var request = new RepairRequestBuilder().Build();
        request.ProvideEstimation("Diagnosis", 100m);
        request.ClearDomainEvents();

        // Act
        var result = request.AcceptEstimation();

        // Assert
        result.HasError().Should().BeFalse();
        request.Status.Should().Be(ERepairRequestStatus.Accepted);
        request.GetDomainEvents().Should().ContainSingle(e => e is RepairRequestAcceptedDomainEvent);
    }

    [Fact]
    public void AcceptEstimation_Should_ReturnError_WhenStatusIsPending()
    {
        // Arrange
        var request = new RepairRequestBuilder().Build();

        // Act
        var result = request.AcceptEstimation();

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        request.Status.Should().Be(ERepairRequestStatus.Pending);
    }

    [Fact]
    public void RejectEstimation_Should_UpdateStateToRejected_WhenStatusIsEstimated()
    {
        // Arrange
        var request = new RepairRequestBuilder().Build();
        request.ProvideEstimation("Diagnosis", 100m);
        request.ClearDomainEvents();
        var reason = "Too expensive";

        // Act
        var result = request.RejectEstimation(reason);

        // Assert
        result.HasError().Should().BeFalse();
        request.Status.Should().Be(ERepairRequestStatus.Rejected);
        request.RejectionReason.Should().Be(reason);
        request.GetDomainEvents().Should().ContainSingle(e => e is RepairRequestRejectedDomainEvent);
    }

    [Fact]
    public void RejectEstimation_Should_ReturnError_WhenReasonIsEmpty()
    {
        // Arrange
        var request = new RepairRequestBuilder().Build();
        request.ProvideEstimation("Diagnosis", 100m);

        // Act
        var result = request.RejectEstimation("");

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
    }
}

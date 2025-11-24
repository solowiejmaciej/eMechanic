
namespace eMechanic.Application.Tests.RepairRequest.Features.Reject;

using Application.Abstractions.Identity.Contexts;
using Application.RepairRequest.Features.Reject;
using Application.RepairRequest.Repositories;
using Application.Vehicle.Vehicle.Services;
using Domain.Tests.Builders;
using Common.Result;
using Domain.Vehicle;
using Domain.Vehicle.Vehicle;
using FluentAssertions;
using NSubstitute;

public class RejectRepairEstimationHandlerTests
{
    private readonly IRepairRequestRepository _repairRequestRepository = Substitute.For<IRepairRequestRepository>();
    private readonly IVehicleOwnershipService _vehicleOwnershipService = Substitute.For<IVehicleOwnershipService>();
    private readonly RejectRepairEstimationHandler _handler;

    private readonly Guid _userId = Guid.NewGuid();
    private readonly Vehicle _vehicle;

    public RejectRepairEstimationHandlerTests()
    {
        _handler = new RejectRepairEstimationHandler(_repairRequestRepository, _vehicleOwnershipService);
        _vehicle = new VehicleBuilder().WithOwnerId(_userId).Build();
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenRequestIsValid()
    {
        // Arrange
        var repairRequest = new RepairRequestBuilder().WithVehicleId(_vehicle.Id).Build();
        repairRequest.ProvideEstimation("test", 1, "aaa");
        var command = new RejectRepairEstimationCommand(repairRequest.Id, "Too expensive");
        _repairRequestRepository.GetByIdAsync(command.RepairRequestId, Arg.Any<CancellationToken>()).Returns(repairRequest);
        _vehicleOwnershipService.VerifyOwnershipAsync(repairRequest.VehicleId, Arg.Any<CancellationToken>())
            .Returns(Result.Success);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        repairRequest.Status.Should().Be(Domain.RepairRequest.Enums.ERepairRequestStatus.Rejected);
        await _repairRequestRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenRepairRequestDoesNotExist()
    {
        // Arrange
        var command = new RejectRepairEstimationCommand(Guid.NewGuid(), "some reason");
        _repairRequestRepository.GetByIdAsync(command.RepairRequestId, Arg.Any<CancellationToken>()).Returns((Domain.RepairRequest.RepairRequest)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(EErrorCode.NotFoundError);
    }

    [Fact]
    public async Task Handle_ShouldReturnForbidden_WhenUserIsNotOwner()
    {
        // Arrange
        var repairRequest = new RepairRequestBuilder().Build();
        var command = new RejectRepairEstimationCommand(repairRequest.Id, "test");
        _repairRequestRepository.GetByIdAsync(command.RepairRequestId, Arg.Any<CancellationToken>()).Returns(repairRequest);
        _vehicleOwnershipService.VerifyOwnershipAsync(repairRequest.VehicleId, Arg.Any<CancellationToken>())
            .Returns(new Error(EErrorCode.UnauthorizedError, "User is not the owner of the vehicle."));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(EErrorCode.UnauthorizedError);
    }
}

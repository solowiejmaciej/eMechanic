
namespace eMechanic.Application.Tests.RepairRequest.Features.Create;

using Application.Abstractions.Identity.Contexts;
using Application.RepairRequest.Features.Create;
using Application.RepairRequest.Repositories;
using Application.Tests.Builders.RepairRequest;
using Application.Vehicle.Vehicle.Services;
using Domain.Tests.Builders;
using Common.Result;
using Domain.Vehicle;
using Domain.Vehicle.Vehicle;
using FluentAssertions;
using NSubstitute;

public class CreateRepairRequestHandlerTests
{
    private readonly IVehicleOwnershipService _vehicleOwnershipService = Substitute.For<IVehicleOwnershipService>();
    private readonly IRepairRequestRepository _repairRequestRepository = Substitute.For<IRepairRequestRepository>();
    private readonly CreateRepairRequestHandler _handler;

    private readonly Guid _userId = Guid.NewGuid();
    private readonly Vehicle _vehicle;

    public CreateRepairRequestHandlerTests()
    {
        _handler = new CreateRepairRequestHandler(_vehicleOwnershipService, _repairRequestRepository);
        _vehicle = new VehicleBuilder().WithOwnerId(_userId).Build();
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenUserIsOwner()
    {
        // Arrange
        var command = new CreateRepairRequestCommandBuilder().WithVehicleId(_vehicle.Id).Build();
        _vehicleOwnershipService.GetAndVerifyOwnershipAsync(command.VehicleId, Arg.Any<CancellationToken>())
            .Returns(_vehicle);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        await _repairRequestRepository.Received(1).AddAsync(Arg.Any<Domain.RepairRequest.RepairRequest>(), Arg.Any<CancellationToken>());
        await _repairRequestRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnForbiddenError_WhenUserIsNotOwner()
    {
        // Arrange
        var command = new CreateRepairRequestCommandBuilder().Build();
        _vehicleOwnershipService.GetAndVerifyOwnershipAsync(command.VehicleId, Arg.Any<CancellationToken>())
            .Returns(new Error(EErrorCode.UnauthorizedError, "User is not the owner of the vehicle."));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Message.Should().Be("User is not the owner of the vehicle.");
        await _repairRequestRepository.DidNotReceive().AddAsync(Arg.Any<Domain.RepairRequest.RepairRequest>(), Arg.Any<CancellationToken>());
        await _repairRequestRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

namespace eMechanic.Application.Tests.Repair.Features.Pay;

using Application.Repair.Features.Pay;
using Application.Repair.Repositories;
using Common.Result;
using Domain.Repair.Enums;
using Domain.Tests.Builders;
using FluentAssertions;
using NSubstitute;

public class PayForRepairCommandHandlerTests
{
    private readonly IRepairRepository _repairRepository = Substitute.For<IRepairRepository>();
    private readonly PayForRepairCommandHandler _handler;

    public PayForRepairCommandHandlerTests()
    {
        _handler = new PayForRepairCommandHandler(_repairRepository);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenRepairExistsAndCanBePaid()
    {
        // Arrange
        var repair = new RepairBuilder().WithStatus(ERepairStatus.Completed).Build();
        var command = new PayForRepairCommand(repair.Id);
        _repairRepository.GetByIdAsync(command.RepairId, Arg.Any<CancellationToken>()).Returns(repair);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        repair.Status.Should().Be(ERepairStatus.Paid);
        await _repairRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_WhenRepairDoesNotExist()
    {
        // Arrange
        var command = new PayForRepairCommand(Guid.NewGuid());
        _repairRepository.GetByIdAsync(command.RepairId, Arg.Any<CancellationToken>()).Returns((Domain.Repair.Repair)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(EErrorCode.NotFoundError);
        await _repairRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnValidationError_WhenRepairCannotBePaidFromCurrentState()
    {
        // Arrange
        var repair = new RepairBuilder().WithStatus(ERepairStatus.Scheduled).Build();
        var command = new PayForRepairCommand(repair.Id);
        _repairRepository.GetByIdAsync(command.RepairId, Arg.Any<CancellationToken>()).Returns(repair);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        repair.Status.Should().Be(ERepairStatus.Scheduled);
        await _repairRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}


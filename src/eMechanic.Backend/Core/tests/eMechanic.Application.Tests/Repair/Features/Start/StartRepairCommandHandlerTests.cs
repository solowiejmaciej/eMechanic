namespace eMechanic.Application.Tests.Repair.Features.Start;

using Application.Abstractions.Identity.Contexts;
using Application.Repair.Features.Start;
using Application.Repair.Repositories;
using Common.Result;
using Domain.Repair.Enums;
using Domain.Tests.Builders;
using FluentAssertions;
using NSubstitute;

public class StartRepairCommandHandlerTests
{
    private readonly IRepairRepository _repairRepository = Substitute.For<IRepairRepository>();
    private readonly IWorkshopContext _workshopContext = Substitute.For<IWorkshopContext>();
    private readonly StartRepairCommandHandler _handler;
    private readonly Guid _workshopId = Guid.NewGuid();

    public StartRepairCommandHandlerTests()
    {
        _handler = new StartRepairCommandHandler(_repairRepository, _workshopContext);
        _workshopContext.GetWorkshopId().Returns(_workshopId);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenRepairExistsAndBelongsToWorkshop()
    {
        // Arrange
        var repair = new RepairBuilder().WithWorkshopId(_workshopId).WithStatus(ERepairStatus.Scheduled).Build();
        var command = new StartRepairCommand(repair.Id);
        _repairRepository.GetByIdAsync(command.RepairId, Arg.Any<CancellationToken>()).Returns(repair);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        repair.Status.Should().Be(ERepairStatus.InProgress);
        await _repairRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnUnauthorized_WhenRepairBelongsToAnotherWorkshop()
    {
        // Arrange
        var repair = new RepairBuilder().WithWorkshopId(Guid.NewGuid()).Build();
        var command = new StartRepairCommand(repair.Id);
        _repairRepository.GetByIdAsync(command.RepairId, Arg.Any<CancellationToken>()).Returns(repair);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(EErrorCode.UnauthorizedError);
        await _repairRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}


namespace eMechanic.Application.Tests.Repair.Features.Complete;

using Application.Abstractions.Identity.Contexts;
using Application.Repair.Features.Complete;
using Application.Repair.Repositories;
using Common.Result;
using Domain.Repair.Enums;
using Domain.Tests.Builders;
using FluentAssertions;
using NSubstitute;

public class CompleteRepairCommandHandlerTests
{
    private readonly IRepairRepository _repairRepository = Substitute.For<IRepairRepository>();
    private readonly IWorkshopContext _workshopContext = Substitute.For<IWorkshopContext>();
    private readonly CompleteRepairCommandHandler _handler;
    private readonly Guid _workshopId = Guid.NewGuid();

    public CompleteRepairCommandHandlerTests()
    {
        _handler = new CompleteRepairCommandHandler(_repairRepository, _workshopContext);
        _workshopContext.GetWorkshopId().Returns(_workshopId);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenRepairIsInProgressAndBelongsToWorkshop()
    {
        // Arrange
        var repair = new RepairBuilder().WithWorkshopId(_workshopId).WithStatus(ERepairStatus.InProgress).Build();
        var command = new CompleteRepairCommand(repair.Id, 1500m, "PLN");
        _repairRepository.GetByIdAsync(command.RepairId, Arg.Any<CancellationToken>()).Returns(repair);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        repair.Status.Should().Be(ERepairStatus.Completed);
        repair.FinalCost.Should().NotBeNull();
        repair.FinalCost!.Amount.Should().Be(1500m);
        repair.FinalCost.Currency.Should().Be("PLN");
        await _repairRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnUnauthorized_WhenRepairBelongsToAnotherWorkshop()
    {
        // Arrange
        var repair = new RepairBuilder().WithWorkshopId(Guid.NewGuid()).WithStatus(ERepairStatus.InProgress).Build();
        var command = new CompleteRepairCommand(repair.Id, 500m, "PLN");
        _repairRepository.GetByIdAsync(command.RepairId, Arg.Any<CancellationToken>()).Returns(repair);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(EErrorCode.UnauthorizedError);
        await _repairRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}


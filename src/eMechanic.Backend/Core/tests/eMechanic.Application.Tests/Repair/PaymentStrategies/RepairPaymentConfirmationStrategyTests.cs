namespace eMechanic.Application.Tests.Repair.PaymentStrategies;

using Application.Repair.Repositories;
using Domain.Repair.Enums;
using Domain.Tests.Builders;
using FluentAssertions;
using NSubstitute;
using eMechanic.Application.Repair.PaymentStrategies;

public class RepairPaymentConfirmationStrategyTests
{
    private readonly IRepairRepository _repairRepository = Substitute.For<IRepairRepository>();
    private readonly RepairPaymentConfirmationStrategy _strategy;

    public RepairPaymentConfirmationStrategyTests()
    {
        _strategy = new RepairPaymentConfirmationStrategy(_repairRepository);
    }

    [Fact]
    public async Task HandleAsync_Should_MarkRepairAsPaid_WhenRepairIsCompleted()
    {
        // Arrange
        var repair = new RepairBuilder().WithStatus(ERepairStatus.Completed).Build();
        _repairRepository.GetByIdAsync(repair.Id, Arg.Any<CancellationToken>()).Returns(repair);

        // Act
        await _strategy.HandleAsync(repair.Id, CancellationToken.None);

        // Assert
        repair.Status.Should().Be(ERepairStatus.Paid);
        await _repairRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_Should_DoNothing_WhenRepairDoesNotExist()
    {
        // Arrange
        _repairRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Domain.Repair.Repair?)null);

        // Act
        await _strategy.HandleAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        await _repairRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_Should_DoNothing_WhenRepairIsNotInCompletedStatus()
    {
        // Arrange
        var repair = new RepairBuilder().WithStatus(ERepairStatus.InProgress).Build();
        _repairRepository.GetByIdAsync(repair.Id, Arg.Any<CancellationToken>()).Returns(repair);

        // Act
        await _strategy.HandleAsync(repair.Id, CancellationToken.None);

        // Assert
        repair.Status.Should().Be(ERepairStatus.InProgress);
        await _repairRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}


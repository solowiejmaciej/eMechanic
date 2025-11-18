
namespace eMechanic.Application.Tests.RepairRequest.Features.ProvideEstimation;

using Application.Abstractions.Identity.Contexts;
using Application.RepairRequest.Features.ProvideEstimation;
using Application.RepairRequest.Repositories;
using Common.Result;
using Domain.Tests.Builders;
using FluentAssertions;
using NSubstitute;

public class ProvideRepairEstimationHandlerTests
{
    private readonly IWorkshopContext _workshopContext = Substitute.For<IWorkshopContext>();
    private readonly IRepairRequestRepository _repairRequestRepository = Substitute.For<IRepairRequestRepository>();
    private readonly ProvideRepairEstimationHandler _handler;

    private readonly Guid _workshopId = Guid.NewGuid();

    public ProvideRepairEstimationHandlerTests()
    {
        _handler = new ProvideRepairEstimationHandler(_workshopContext, _repairRequestRepository);
        _workshopContext.GetWorkshopId().Returns(_workshopId);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenRequestIsValid()
    {
        // Arrange
        var repairRequest = new RepairRequestBuilder().WithWorkshopId(_workshopId).Build();
        var command = new ProvideRepairEstimationCommand(repairRequest.Id, "New diagnosis", 500, "USD");
        _repairRequestRepository.GetByIdAsync(repairRequest.Id, Arg.Any<CancellationToken>()).Returns(repairRequest);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        repairRequest.Status.Should().Be(Domain.RepairRequest.Enums.ERepairRequestStatus.Estimated);
        repairRequest.Diagnosis!.Value.Should().Be("New diagnosis");
        await _repairRequestRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenRepairRequestDoesNotExist()
    {
        // Arrange
        var command = new ProvideRepairEstimationCommand(Guid.NewGuid(), "Diagnosis", 100, "EUR");
        _repairRequestRepository.GetByIdAsync(command.RepairRequestId, Arg.Any<CancellationToken>()).Returns((Domain.RepairRequest.RepairRequest)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(EErrorCode.NotFoundError);
    }

    [Fact]
    public async Task Handle_ShouldReturnForbidden_WhenWorkshopIsNotAssigned()
    {
        // Arrange
        var otherWorkshopId = Guid.NewGuid();
        var repairRequest = new RepairRequestBuilder().WithWorkshopId(otherWorkshopId).Build();
        var command = new ProvideRepairEstimationCommand(repairRequest.Id, "Diagnosis", 100, "EUR");
        _repairRequestRepository.GetByIdAsync(repairRequest.Id, Arg.Any<CancellationToken>()).Returns(repairRequest);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(EErrorCode.UnauthorizedError);
    }
}

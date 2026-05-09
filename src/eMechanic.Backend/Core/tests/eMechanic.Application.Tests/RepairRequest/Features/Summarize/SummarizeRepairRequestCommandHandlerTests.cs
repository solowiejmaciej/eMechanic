namespace eMechanic.Application.Tests.RepairRequest.Features.Summarize;

using System;
using System.Threading;
using System.Threading.Tasks;
using eMechanic.Application.Abstractions.Identity.Contexts;
using eMechanic.Application.RepairRequest.Features.Summarize;
using eMechanic.Application.RepairRequest.Repositories;
using eMechanic.Application.RepairRequest.Services;
using eMechanic.Application.Tests.Builders.RepairRequest;
using eMechanic.Common.Result;
using eMechanic.Domain.Tests.Builders;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class SummarizeRepairRequestCommandHandlerTests
{
    private readonly IRepairRequestRepository _repairRequestRepository;
    private readonly IRepairRequestSummaryService _summaryService;
    private readonly IUserContext _userContext;
    private readonly SummarizeRepairRequestCommandHandler _sut;

    public SummarizeRepairRequestCommandHandlerTests()
    {
        _repairRequestRepository = Substitute.For<IRepairRequestRepository>();
        _summaryService = Substitute.For<IRepairRequestSummaryService>();
        _userContext = Substitute.For<IUserContext>();
        _sut = new SummarizeRepairRequestCommandHandler(_summaryService, _repairRequestRepository, _userContext);
    }

    [Fact]
    public async Task Handle_ShouldReturnSummary_WhenRepairRequestExists()
    {
        // Arrange
        var command = new SummarizeRepairRequestCommandBuilder().Build();
        var repairRequest = new RepairRequestBuilder().Build();
        var summary = "This is a summary.";
        _userContext.GetUserId().Returns(repairRequest.UserId);
        _repairRequestRepository.GetForUserByIdAsync(repairRequest.UserId, command.RepairRequestId, CancellationToken.None).Returns(repairRequest);
        _summaryService.GenerateSummaryReport(repairRequest, CancellationToken.None).Returns(summary);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(summary);
        repairRequest.SummaryReport.Should().Be(summary);
        await _repairRequestRepository.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenRepairRequestNotFound()
    {
        // Arrange
        var command = new SummarizeRepairRequestCommandBuilder().Build();
        var userId = Guid.NewGuid();
        _userContext.GetUserId().Returns(userId);
        _repairRequestRepository.GetForUserByIdAsync(userId, command.RepairRequestId, CancellationToken.None).Returns(Task.FromResult<Domain.RepairRequest.RepairRequest?>(null));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(EErrorCode.NotFoundError);
        await _summaryService.DidNotReceive().GenerateSummaryReport(Arg.Any<Domain.RepairRequest.RepairRequest>(), CancellationToken.None);
        await _repairRequestRepository.DidNotReceive().SaveChangesAsync(CancellationToken.None);
    }
}

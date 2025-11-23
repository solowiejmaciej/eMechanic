using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using eMechanic.Application.Abstractions.Identity.Contexts;
using eMechanic.Application.RepairRequest.Features.Get.ForWorkshop;
using eMechanic.Application.RepairRequest.Repositories;
using eMechanic.Application.Tests.Builders.RepairRequest;
using eMechanic.Common.Result;
using eMechanic.Domain.Tests.Builders;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace eMechanic.Application.Tests.RepairRequest;

public class GetRepairRequestsForWorkshopHandlerTests
{
    private readonly IRepairRequestRepository _repairRequestRepository;
    private readonly IWorkshopContext _workshopContext;
    private readonly GetRepairRequestsForWorkshopHandler _sut;

    public GetRepairRequestsForWorkshopHandlerTests()
    {
        _repairRequestRepository = Substitute.For<IRepairRequestRepository>();
        _workshopContext = Substitute.For<IWorkshopContext>();
        _sut = new GetRepairRequestsForWorkshopHandler(_workshopContext, _repairRequestRepository);
    }

    [Fact]
    public async Task Handle_ShouldReturnRepairRequests_WhenWorkshopExists()
    {
        // Arrange
        var query = new GetRepairRequestsForWorkshopQueryBuilder().Build();
        var workshopId = Guid.NewGuid();
        var repairRequest = new RepairRequestBuilder().WithWorkshopId(workshopId).Build();
        var repairRequests = new List<Domain.RepairRequest.RepairRequest> { repairRequest };
        var paginationResult = new PaginationResult<Domain.RepairRequest.RepairRequest>(repairRequests, 1, 1, 10);
        
        _workshopContext.GetWorkshopId().Returns(workshopId);
        _repairRequestRepository.GetForWorkshopAsync(workshopId, query.Pagination, CancellationToken.None).Returns(paginationResult);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Items.Should().HaveCount(1);
    }
}
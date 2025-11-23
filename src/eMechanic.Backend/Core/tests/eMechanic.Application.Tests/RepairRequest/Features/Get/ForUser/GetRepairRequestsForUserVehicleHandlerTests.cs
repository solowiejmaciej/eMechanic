using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using eMechanic.Application.RepairRequest.Features.Get;
using eMechanic.Application.RepairRequest.Features.Get.ForUser;
using eMechanic.Application.RepairRequest.Repositories;
using eMechanic.Application.Tests.Builders.RepairRequest;
using eMechanic.Application.Vehicle.Services;
using eMechanic.Common.Result;
using eMechanic.Domain.Tests.Builders;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace eMechanic.Application.Tests.RepairRequest;

public class GetRepairRequestsForUserVehicleHandlerTests
{
    private readonly IRepairRequestRepository _repairRequestRepository;
    private readonly IVehicleOwnershipService _vehicleOwnershipService;
    private readonly GetRepairRequestsForUserVehicleHandler _sut;

    public GetRepairRequestsForUserVehicleHandlerTests()
    {
        _repairRequestRepository = Substitute.For<IRepairRequestRepository>();
        _vehicleOwnershipService = Substitute.For<IVehicleOwnershipService>();
        _sut = new GetRepairRequestsForUserVehicleHandler(_vehicleOwnershipService, _repairRequestRepository);
    }

    [Fact]
    public async Task Handle_ShouldReturnRepairRequests_WhenVehicleExistsAndUserIsOwner()
    {
        // Arrange
        var query = new GetRepairRequestsForUserVehicleQueryBuilder().Build();
        var repairRequest = new RepairRequestBuilder().Build();
        var repairRequests = new List<Domain.RepairRequest.RepairRequest> { repairRequest };
        var paginationResult = new PaginationResult<Domain.RepairRequest.RepairRequest>(repairRequests, 1, 1, 10);
        
        _vehicleOwnershipService.VerifyOwnershipAsync(query.VehicleId, CancellationToken.None).Returns(Result.Success);
        _repairRequestRepository.GetForUserVehicleAsync(query.VehicleId, query.Pagination, CancellationToken.None).Returns(paginationResult);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenOwnershipVerificationFails()
    {
        // Arrange
        var query = new GetRepairRequestsForUserVehicleQueryBuilder().Build();
        _vehicleOwnershipService.VerifyOwnershipAsync(query.VehicleId, CancellationToken.None).Returns(new Error(EErrorCode.UnauthorizedError, "Test"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(EErrorCode.UnauthorizedError);
    }
}
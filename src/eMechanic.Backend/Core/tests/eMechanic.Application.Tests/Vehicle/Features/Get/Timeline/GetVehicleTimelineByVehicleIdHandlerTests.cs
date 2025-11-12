namespace eMechanic.Application.Tests.Vehicle.Features.Get.Timeline;

using System.Text.Json;
using Application.Vehicle.Features.Get.Timeline;
using Application.Vehicle.Repostories;
using Application.Vehicle.Services;
using Common.Result;
using Domain.Tests.Builders;
using Domain.Vehicle;
using Domain.Vehicle.Enums;
using Domain.VehicleTimeline;
using FluentAssertions;
using NSubstitute;

public class GetVehicleTimelineByVehicleIdHandlerTests
{
    private readonly IVehicleTimelineRepository _vehicleTimelineRepository;
    private readonly IVehicleOwnershipService _vehicleOwnershipService;
    private readonly GetVehicleTimelineByVehicleIdHandler _handler;

    private readonly Guid _vehicleId = Guid.NewGuid();
    private readonly Guid _currentUserId = Guid.NewGuid();
    private readonly Vehicle _fakeVehicle;

    public GetVehicleTimelineByVehicleIdHandlerTests()
    {
        _vehicleTimelineRepository = Substitute.For<IVehicleTimelineRepository>();
        _vehicleOwnershipService = Substitute.For<IVehicleOwnershipService>();
        _handler = new GetVehicleTimelineByVehicleIdHandler(_vehicleTimelineRepository, _vehicleOwnershipService);

        var creationResult = new VehicleBuilder().WithOwnerId(_currentUserId).BuildResult();

        creationResult.HasError().Should().BeFalse();
        _fakeVehicle = creationResult.Value!;
        typeof(Vehicle).GetProperty("Id")!.SetValue(_fakeVehicle, _vehicleId);
    }

    [Fact]
    public async Task Handle_Should_ReturnPaginatedTimeline_WhenOwnershipIsVerified()
    {
        var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10 };
        var query = new GetVehicleTimelineByVehicleIdQuery(_vehicleId, parameters);

        var timelineEvents = new List<VehicleTimeline>
        {
            VehicleTimeline.Create(_vehicleId, "TestEvent1", JsonSerializer.Serialize(new { Info = "A" })),
            VehicleTimeline.Create(_vehicleId, "TestEvent2", JsonSerializer.Serialize(new { Info = "B" }))
        };
        var paginatedResult = new PaginationResult<VehicleTimeline>(timelineEvents, 2, 1, 10);

        _vehicleOwnershipService.GetAndVerifyOwnershipAsync(_vehicleId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Result<Vehicle, Error>>(_fakeVehicle));

        _vehicleTimelineRepository.GetByVehicleIdPaginatedAsync(_vehicleId, parameters, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(paginatedResult));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Items.Count().Should().Be(2);
        result.Value.TotalCount.Should().Be(2);
        result.Value.Items.First().EventType.Should().Be("TestEvent1");
        await _vehicleTimelineRepository.Received(1).GetByVehicleIdPaginatedAsync(_vehicleId, parameters, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenOwnershipServiceFails()
    {
        var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10 };
        var query = new GetVehicleTimelineByVehicleIdQuery(_vehicleId, parameters);

        var notFoundError = new Error(EErrorCode.NotFoundError, "Vehicle not found or user is not owner.");
        _vehicleOwnershipService.GetAndVerifyOwnershipAsync(_vehicleId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Result<Vehicle, Error>>(notFoundError));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.HasError().Should().BeTrue();
        result.Error.Should().Be(notFoundError);
        await _vehicleTimelineRepository.DidNotReceiveWithAnyArgs().GetByVehicleIdPaginatedAsync(default, default!, default);
    }
}

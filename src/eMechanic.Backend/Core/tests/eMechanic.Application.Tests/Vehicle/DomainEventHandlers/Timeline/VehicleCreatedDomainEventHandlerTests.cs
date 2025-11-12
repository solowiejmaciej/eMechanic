namespace eMechanic.Application.Tests.Vehicle.DomainEventHandlers.Timeline;

using Application.Vehicle.Repostories;
using eMechanic.Application.Vehicle.DomainEventHandlers.Timeline;
using eMechanic.Domain.Tests.Builders;
using eMechanic.Domain.Vehicle;
using eMechanic.Domain.Vehicle.Enums;
using eMechanic.Domain.VehicleTimeline;
using FluentAssertions;
using NSubstitute;

public class VehicleCreatedDomainEventHandlerTests
{
    private readonly IVehicleTimelineRepository _vehicleTimelineRepository;
    private readonly VehicleCreatedDomainEventHandler _handler;

    public VehicleCreatedDomainEventHandlerTests()
    {
        _vehicleTimelineRepository = Substitute.For<IVehicleTimelineRepository>();
        _handler = new VehicleCreatedDomainEventHandler(_vehicleTimelineRepository);
    }

    [Fact]
    public async Task Handle_Should_CreateTimelineEntry_WhenVehicleHasAllData()
    {
        // Arrange
        var vehicleResult = new VehicleBuilder().WithEngineCapacity(1.8m).BuildResult();
        vehicleResult.HasError().Should().BeFalse();
        var vehicle = vehicleResult.Value;
        var notification = new VehicleCreatedDomainEvent(vehicle!);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await _vehicleTimelineRepository.Received(1).AddAsync(
            Arg.Any<VehicleTimeline>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_CreateTimelineEntry_WhenVehicleHasNullEngineCapacity()
    {
        // Arrange
        var vehicleResult = new VehicleBuilder().WithEngineCapacity(null).BuildResult();
        vehicleResult.HasError().Should().BeFalse();
        var vehicle = vehicleResult.Value;
        var notification = new VehicleCreatedDomainEvent(vehicle!);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await _vehicleTimelineRepository.Received(1).AddAsync(
            Arg.Is<VehicleTimeline>(entry =>
                entry.VehicleId == vehicle!.Id &&
                entry.EventType == nameof(VehicleCreatedDomainEvent) &&
                entry.Data.Contains(vehicle.Vin.Value) &&
                entry.Data.Contains(vehicle.Model.Value) &&
                entry.Data.Contains("\"EngineCapacity\":null")
            ),
            Arg.Any<CancellationToken>());
    }
}

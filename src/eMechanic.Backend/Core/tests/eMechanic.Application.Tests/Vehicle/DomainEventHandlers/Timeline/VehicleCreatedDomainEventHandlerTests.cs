namespace eMechanic.Application.Tests.Vehicle.DomainEventHandlers.Timeline;

using eMechanic.Application.Abstractions.VehicleTimeline;
using eMechanic.Application.Vehicle.DomainEventHandlers.Timeline;
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

    private Vehicle CreateTestVehicle(decimal? engineCapacity)
    {
        var creationResult = Vehicle.Create(
            Guid.NewGuid(),
            "V1N123456789ABCDE",
            "Test Manufacturer",
            "Test Model",
            "2022",
            engineCapacity,
            10000,
            EMileageUnit.Kilometers,
            "PZ1W924",
            124,
            EFuelType.Gasoline,
            EBodyType.Sedan,
            EVehicleType.Passenger);

        creationResult.HasError().Should().BeFalse();
        return creationResult.Value!;
    }

    [Fact]
    public async Task Handle_Should_CreateTimelineEntry_WhenVehicleHasAllData()
    {
        // Arrange
        var vehicle = CreateTestVehicle(1.8m);
        var notification = new VehicleCreatedDomainEvent(vehicle);

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
        var vehicle = CreateTestVehicle(null);
        var notification = new VehicleCreatedDomainEvent(vehicle);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await _vehicleTimelineRepository.Received(1).AddAsync(
            Arg.Is<VehicleTimeline>(entry =>
                entry.VehicleId == vehicle.Id &&
                entry.EventType == nameof(VehicleCreatedDomainEvent) &&
                entry.Data.Contains(vehicle.Vin.Value) &&
                entry.Data.Contains(vehicle.Model.Value) &&
                entry.Data.Contains("\"EngineCapacity\":null")
            ),
            Arg.Any<CancellationToken>());
    }
}

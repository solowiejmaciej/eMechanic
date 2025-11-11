namespace eMechanic.Application.Tests.Vehicle.DomainEventHandlers.Timeline;

using System.Threading;
using System.Threading.Tasks;
using Application.Vehicle.DomainEventHandlers;
using Application.Vehicle.DomainEventHandlers.Timeline;
using Application.Vehicle.Repostories;
using Domain.Vehicle;
using Domain.Vehicle.Enums;
using Domain.Vehicle.ValueObjects;
using Domain.VehicleTimeline;
using FluentAssertions;
using NSubstitute;

public class VehicleUpdateDomainEventHandlersTests
{
    private readonly IVehicleTimelineRepository _vehicleTimelineRepository;
    private readonly Guid _vehicleId = Guid.NewGuid();

    public VehicleUpdateDomainEventHandlersTests()
    {
        _vehicleTimelineRepository = Substitute.For<IVehicleTimelineRepository>();
    }

    private void AssertTimelineEntryWasAdded(string eventName, string expectedJsonSnippet)
    {
        _vehicleTimelineRepository.Received(1).AddAsync(
            Arg.Is<VehicleTimeline>(entry =>
                entry.VehicleId == _vehicleId &&
                entry.EventType == eventName &&
                entry.Data.Contains(expectedJsonSnippet)
            ),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_VehicleManufacturerChanged_Should_CreateTimelineEntry()
    {
        // Arrange
        var handler = new VehicleManufacturerChangedDomainEventHandler(_vehicleTimelineRepository);
        var oldVal = Manufacturer.Create("Old").Value!;
        var newVal = Manufacturer.Create("New").Value!;
        var notification = new VehicleManufacturerChangedDomainEvent(_vehicleId, oldVal, newVal);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        AssertTimelineEntryWasAdded(nameof(VehicleManufacturerChangedDomainEvent), "\"Manufacturer\":{\"OldValue\":{\"Value\":\"Old\"},\"NewValue\":{\"Value\":\"New\"}}");
    }

    [Fact]
    public async Task Handle_VehicleModelChanged_Should_CreateTimelineEntry()
    {
        // Arrange
        var handler = new VehicleModelChangedDomainEventHandler(_vehicleTimelineRepository);
        var oldVal = Model.Create("Old").Value!;
        var newVal = Model.Create("New").Value!;
        var notification = new VehicleModelChangedDomainEvent(_vehicleId, oldVal, newVal);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        AssertTimelineEntryWasAdded(nameof(VehicleModelChangedDomainEvent), "\"Model\":{\"OldValue\":{\"Value\":\"Old\"},\"NewValue\":{\"Value\":\"New\"}}");
    }

    [Fact]
    public async Task Handle_VehicleProductionYearChanged_Should_CreateTimelineEntry()
    {
        // Arrange
        var handler = new VehicleProductionYearChangedDomainEventHandler(_vehicleTimelineRepository);
        var oldVal = ProductionYear.Create("2000").Value!;
        var newVal = ProductionYear.Create("2010").Value!;
        var notification = new VehicleProductionYearChangedDomainEvent(_vehicleId, oldVal, newVal);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        AssertTimelineEntryWasAdded(nameof(VehicleProductionYearChangedDomainEvent), "\"ProductionYear\":{\"OldValue\":{\"Value\":\"2000\"},\"NewValue\":{\"Value\":\"2010\"}}");
    }

    [Fact]
    public async Task Handle_VehicleVinChanged_Should_CreateTimelineEntry()
    {
        // Arrange
        var handler = new VehicleVinChangedDomainEventHandler(_vehicleTimelineRepository);
        var oldVal = Vin.Create("V1N123456789ABCDE").Value!;
        var newVal = Vin.Create("V1N123456789ABCDF").Value!;
        var notification = new VehicleVinChangedDomainEvent(_vehicleId, oldVal, newVal);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        AssertTimelineEntryWasAdded(nameof(VehicleVinChangedDomainEvent), "\"Vin\":{\"OldValue\":{\"Value\":\"V1N123456789ABCDE\"},\"NewValue\":{\"Value\":\"V1N123456789ABCDF\"}}");
    }

    [Fact]
    public async Task Handle_VehicleUserIdChanged_Should_CreateTimelineEntry()
    {
        // Arrange
        var handler = new VehicleUserIdChangedDomainEventHandler(_vehicleTimelineRepository);
        var oldVal = Guid.NewGuid();
        var newVal = Guid.NewGuid();
        var notification = new VehicleUserIdChangedDomainEvent(_vehicleId, oldVal, newVal);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        AssertTimelineEntryWasAdded(nameof(VehicleUserIdChangedDomainEvent), $"\"UserId\":{{\"OldValue\":\"{oldVal}\",\"NewValue\":\"{newVal}\"}}");
    }

    [Fact]
    public async Task Handle_VehicleBodyTypeChanged_Should_CreateTimelineEntry()
    {
        // Arrange
        var handler = new VehicleBodyTypeChangedDomainEventHandler(_vehicleTimelineRepository);
        var oldVal = EBodyType.Sedan;
        var newVal = EBodyType.Kombi;
        var notification = new VehicleBodyTypeChangedDomainEvent(_vehicleId, oldVal, newVal);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        AssertTimelineEntryWasAdded(nameof(VehicleBodyTypeChangedDomainEvent), "\"BodyType\":{\"OldValue\":1,\"NewValue\":3}");
    }

    [Fact]
    public async Task Handle_VehicleFuelTypeChanged_Should_CreateTimelineEntry()
    {
        // Arrange
        var handler = new VehicleFuelTypeChangedDomainEventHandler(_vehicleTimelineRepository);
        var oldVal = EFuelType.Gasoline;
        var newVal = EFuelType.Diesel;
        var notification = new VehicleFuelTypeChangedDomainEvent(_vehicleId, oldVal, newVal);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        AssertTimelineEntryWasAdded(nameof(VehicleFuelTypeChangedDomainEvent), "\"FuelType\":{\"OldValue\":1,\"NewValue\":2}");
    }

    [Fact]
    public async Task Handle_VehicleTypeChanged_Should_CreateTimelineEntry()
    {
        // Arrange
        var handler = new VehicleTypeChangedDomainEventHandler(_vehicleTimelineRepository);
        var oldVal = EVehicleType.Passenger;
        var newVal = EVehicleType.Motorcycle;
        var notification = new VehicleTypeChangedDomainEvent(_vehicleId, oldVal, newVal);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        AssertTimelineEntryWasAdded(nameof(VehicleTypeChangedDomainEvent), "\"VehicleType\":{\"OldValue\":1,\"NewValue\":2}");
    }

    [Fact]
    public async Task Handle_VehicleMileageChanged_Should_CreateTimelineEntry()
    {
        // Arrange
        var handler = new VehicleMileageChangedDomainEventHandler(_vehicleTimelineRepository);
        var oldVal = Mileage.Create(10000, EMileageUnit.Kilometers).Value!;
        var newVal = Mileage.Create(15000, EMileageUnit.Kilometers).Value!;
        var notification = new VehicleMileageChangedDomainEvent(_vehicleId, oldVal, newVal);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        AssertTimelineEntryWasAdded(nameof(VehicleMileageChangedDomainEvent), "\"Mileage\":{\"OldValue\":{\"Value\":10000,\"Unit\":1},\"NewValue\":{\"Value\":15000,\"Unit\":1}}");
    }

    [Fact]
    public async Task Handle_VehicleEngineCapacityChanged_Should_CreateTimelineEntry_WhenBothValuesExist()
    {
        // Arrange
        var handler = new VehicleEngineCapacityChangedDomainEventHandler(_vehicleTimelineRepository);
        var oldVal = EngineCapacity.Create(1.8m).Value!;
        var newVal = EngineCapacity.Create(2.0m).Value!;
        var notification = new VehicleEngineCapacityChangedDomainEvent(_vehicleId, oldVal, newVal);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        AssertTimelineEntryWasAdded(nameof(VehicleEngineCapacityChangedDomainEvent), "\"EngineCapacity\":{\"OldValue\":{\"Value\":1.8},\"NewValue\":{\"Value\":2.0}}");
    }

    [Fact]
    public async Task Handle_VehicleEngineCapacityChanged_Should_CreateTimelineEntry_WhenNewValueIsNull()
    {
        // Arrange
        var handler = new VehicleEngineCapacityChangedDomainEventHandler(_vehicleTimelineRepository);
        var oldVal = EngineCapacity.Create(1.8m).Value!;
        var notification = new VehicleEngineCapacityChangedDomainEvent(_vehicleId, oldVal, null);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        AssertTimelineEntryWasAdded(nameof(VehicleEngineCapacityChangedDomainEvent), "\"EngineCapacity\":{\"OldValue\":{\"Value\":1.8},\"NewValue\":null}");
    }

    [Fact]
    public async Task Handle_VehicleEngineCapacityChanged_Should_CreateTimelineEntry_WhenOldValueIsNull()
    {
        // Arrange
        var handler = new VehicleEngineCapacityChangedDomainEventHandler(_vehicleTimelineRepository);
        var newVal = EngineCapacity.Create(2.5m).Value!;
        var notification = new VehicleEngineCapacityChangedDomainEvent(_vehicleId, null, newVal);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        AssertTimelineEntryWasAdded(nameof(VehicleEngineCapacityChangedDomainEvent), "\"EngineCapacity\":{\"OldValue\":null,\"NewValue\":{\"Value\":2.5}}");
    }

    [Fact]
    public async Task Handle_VehicleLicensePlateChanged_Should_CreateTimelineEntry()
    {
        // Arrange
        var handler = new VehicleLicensePlateChangedDomainEventHandler(_vehicleTimelineRepository);
        var oldVal = LicensePlate.Create("OLD 123").Value!;
        var newVal = LicensePlate.Create("NEW 456").Value!;
        var notification = new VehicleLicensePlateChangedDomainEvent(_vehicleId, oldVal, newVal);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        AssertTimelineEntryWasAdded(nameof(VehicleLicensePlateChangedDomainEvent), "\"LicensePlate\":{\"OldValue\":{\"Value\":\"OLD 123\"},\"NewValue\":{\"Value\":\"NEW 456\"}}");
    }

    [Fact]
    public async Task Handle_VehicleHorsePowerChanged_Should_CreateTimelineEntry()
    {
        // Arrange
        var handler = new VehicleHorsePowerChangedDomainEventHandler(_vehicleTimelineRepository);
        var oldVal = HorsePower.Create(100).Value!;
        var newVal = HorsePower.Create(150).Value!;
        var notification = new VehicleHorsePowerChangedDomainEvent(_vehicleId, oldVal, newVal);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        AssertTimelineEntryWasAdded(nameof(VehicleHorsePowerChangedDomainEvent), "\"HorsePower\":{\"OldValue\":{\"Value\":100},\"NewValue\":{\"Value\":150}}");
    }
}

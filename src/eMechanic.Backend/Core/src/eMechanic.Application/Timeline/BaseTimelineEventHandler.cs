namespace eMechanic.Application.Timeline;

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Domain.VehicleTimeline;
using eMechanic.Application.Abstractions.VehicleTimeline;
using Microsoft.Extensions.Logging;

public abstract class BaseTimelineEventHandler
{
    private readonly IVehicleTimelineRepository _vehicleTimelineRepository;

    protected BaseTimelineEventHandler(IVehicleTimelineRepository vehicleVehicleTimelineRepository)
    {
        _vehicleTimelineRepository = vehicleVehicleTimelineRepository;
    }

    protected async Task CreateTimelineEntryAsync(
        Guid vehicleId,
        string eventType,
        object payload,
        CancellationToken cancellationToken)
    {
        var jsonData = JsonSerializer.Serialize(payload);
        var timelineEntry = VehicleTimeline.Create(vehicleId, eventType, jsonData);
        await _vehicleTimelineRepository.AddAsync(timelineEntry, cancellationToken);
    }
}

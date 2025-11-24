namespace eMechanic.Application.Vehicle.Timeline.Features.Get;

public sealed record VehicleTimelineResponse(string EventType, string Data, DateTime CreatedAt);

namespace eMechanic.Application.Vehicle.Get;

public sealed record VehicleTimelineResponse(string EventType, string Data, DateTime CreatedAt);

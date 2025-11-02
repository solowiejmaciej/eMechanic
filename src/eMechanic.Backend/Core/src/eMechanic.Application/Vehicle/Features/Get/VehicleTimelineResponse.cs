namespace eMechanic.Application.Vehicle.Features.Get;

public sealed record VehicleTimelineResponse(string EventType, string Data, DateTime CreatedAt);

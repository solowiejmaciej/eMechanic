namespace eMechanic.Domain.VehicleTimeline;

using Common.DDD;
using References.Vehicle;

public class VehicleTimeline : AggregateRoot, IVehicleReference
{
    public Guid VehicleId { get; private set; }
    public string EventType { get; private set; }
    public string Data { get; private set; }

    private VehicleTimeline() { }

    private VehicleTimeline(
        Guid vehicleId,
        string eventType,
        string data)
    {
        if (vehicleId == Guid.Empty)
        {
            throw new ArgumentException("VehicleId cannot be empty", nameof(vehicleId));
        }

        if (string.IsNullOrWhiteSpace(eventType))
        {
            throw new ArgumentException("EventType cannot be empty", nameof(eventType));
        }

        if (string.IsNullOrWhiteSpace(data))
        {
            throw new ArgumentException("Data cannot be empty", nameof(data));
        }

        VehicleId = vehicleId;
        EventType = eventType;
        Data = data;
    }

    public static VehicleTimeline Create(Guid vehicleId, string eventType, string message) => new(vehicleId, eventType, message);
}

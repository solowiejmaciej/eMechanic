namespace eMechanic.Events.Events.RepairRequest;

public class RepairRequestRejectedEvent : EventBase
{
    public Guid RepairRequestId { get; private set; }
    public Guid UserId { get; private set; }
    public string UserEmail { get; private set; }
    public string UserPhoneNumber { get; private set; }
    public string UserFirstName { get; private set; }
    public Guid VehicleId { get; private set; }
    public string VehicleVin { get; private set; }
    public string VehicleModel { get; private set; }
    public string VehicleLicensePlate { get; private set; }
    public string VehicleProductionYear { get; private set; }
    public string VehicleManufacturer { get; private set; }
    public string RejectionReason { get; private set; }

    public RepairRequestRejectedEvent(
        Guid repairRequestId,
        Guid userId,
        string userEmail,
        string userPhoneNumber,
        string userFirstName,
        Guid vehicleId,
        string vehicleVin,
        string vehicleModel,
        string vehicleLicensePlate,
        string vehicleProductionYear,
        string vehicleManufacturer,
        string rejectionReason)
    {
        RepairRequestId = repairRequestId;
        UserId = userId;
        UserEmail = userEmail;
        UserPhoneNumber = userPhoneNumber;
        UserFirstName = userFirstName;
        VehicleId = vehicleId;
        VehicleVin = vehicleVin;
        VehicleModel = vehicleModel;
        VehicleLicensePlate = vehicleLicensePlate;
        VehicleProductionYear = vehicleProductionYear;
        VehicleManufacturer = vehicleManufacturer;
        RejectionReason = rejectionReason;
    }
}
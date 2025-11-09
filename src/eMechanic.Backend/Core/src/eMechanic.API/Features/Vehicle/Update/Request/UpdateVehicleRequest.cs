namespace eMechanic.API.Features.Vehicle.Update.Request;

using Application.Vehicle.Features.Update;
using eMechanic.Domain.Vehicle.Enums;

public sealed record UpdateVehicleRequest(
    string Vin,
    string Manufacturer,
    string Model,
    string ProductionYear,
    decimal? EngineCapacity,
    int MileageValue,
    EMileageUnit MileageUnit,
    string LicensePlate,
    int HorsePower,
    EFuelType FuelType,
    EBodyType BodyType,
    EVehicleType VehicleType)
{
    public UpdateVehicleCommand ToCommand(Guid vehicleId) => new(vehicleId, Vin, Manufacturer, Model, ProductionYear, EngineCapacity, MileageValue, MileageUnit, LicensePlate, HorsePower, FuelType, BodyType, VehicleType);
}

namespace eMechanic.API.Features.Vehicle.Update.Request;

using Application.Vehicle.Update;
using eMechanic.Application.Vehicle.Create;
using eMechanic.Domain.Vehicle.Enums;

public sealed record UpdateVehicleRequest(
    string Vin,
    string Manufacturer,
    string Model,
    string ProductionYear,
    decimal? EngineCapacity,
    int MileageValue,
    EMileageUnit MileageUnit,
    EFuelType FuelType,
    EBodyType BodyType,
    EVehicleType VehicleType)
{
    public UpdateVehicleCommand ToCommand(Guid vehicleId) => new(vehicleId, Vin, Manufacturer, Model, ProductionYear, EngineCapacity, MileageValue, MileageUnit, FuelType, BodyType, VehicleType);
}

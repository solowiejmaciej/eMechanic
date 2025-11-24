namespace eMechanic.API.Features.Vehicle.Vehicle.Create.Request;

using Application.Vehicle.Vehicle.Features.Create;
using Domain.Vehicle.Vehicle.Enums;

public sealed record CreateVehicleRequest(
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
    public CreateVehicleCommand ToCommand() => new(Vin, Manufacturer, Model, ProductionYear, EngineCapacity, MileageValue, MileageUnit, LicensePlate, HorsePower, FuelType, BodyType, VehicleType);
}

namespace eMechanic.API.Features.Vehicle.Vehicle.Create.Request;

using eMechanic.Application.Vehicle.Features.Create;
using eMechanic.Domain.Vehicle.Enums;

public sealed record CreateVehicleRequest(
    string Vin,
    string Manufacturer,
    string Model,
    string ProductionYear,
    decimal? EngineCapacity,
    int Mileage,
    EMileageUnit MileageUnit,
    string LicensePlate,
    int HorsePower,
    EFuelType FuelType,
    EBodyType BodyType,
    EVehicleType VehicleType)
{
    public CreateVehicleCommand ToCommand() => new(Vin, Manufacturer, Model, ProductionYear, EngineCapacity, Mileage, MileageUnit, LicensePlate, HorsePower, FuelType, BodyType, VehicleType);
}

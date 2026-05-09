namespace eMechanic.Application.Vehicle.Vehicle.Features.Get;

using Domain.Vehicle.Vehicle.Enums;

public sealed record VehicleResponse(
    Guid Id,
    Guid UserId,
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
    EVehicleType VehicleType,
    DateTime CreatedAt);

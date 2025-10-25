namespace eMechanic.Application.Vehicle.GetById;

using eMechanic.Domain.Vehicle.Enums;

public sealed record VehicleResponse(
    Guid Id,
    Guid OwnerUserId,
    string Vin,
    string Manufacturer,
    string Model,
    string ProductionYear,
    decimal? EngineCapacity,
    EFuelType FuelType,
    EBodyType BodyType,
    EVehicleType VehicleType,
    DateTime CreatedAt);

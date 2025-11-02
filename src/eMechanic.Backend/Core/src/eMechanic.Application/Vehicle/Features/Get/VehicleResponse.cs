namespace eMechanic.Application.Vehicle.Features.Get;

using eMechanic.Domain.Vehicle.Enums;

public sealed record VehicleResponse(
    Guid Id,
    Guid UserId,
    string Vin,
    string Manufacturer,
    string Model,
    string ProductionYear,
    decimal? EngineCapacity,
    int Mileage,
    EMileageUnit MileageUnit,
    EFuelType FuelType,
    EBodyType BodyType,
    EVehicleType VehicleType,
    DateTime CreatedAt);

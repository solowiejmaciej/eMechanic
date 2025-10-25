namespace eMechanic.API.Features.Vehicle.Create.Request;

using Application.Vehicle.Create;
using Domain.Vehicle.Enums;

public sealed record CreateVehicleRequest(
    string Vin,
    string Manufacturer,
    string Model,
    string ProductionYear,
    decimal? EngineCapacity,
    EFuelType FuelType,
    EBodyType BodyType,
    EVehicleType VehicleType)
{
    public CreateVehicleCommand ToCommand()
    {
        return new CreateVehicleCommand(Vin, Manufacturer, Model, ProductionYear, EngineCapacity,
            FuelType, BodyType, VehicleType);
    }
}

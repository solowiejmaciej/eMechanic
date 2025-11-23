
using System;
using eMechanic.Application.Vehicle.Features.Update;
using eMechanic.Domain.Vehicle.Enums;

namespace eMechanic.Application.Tests.Builders.Vehicle;

public class UpdateVehicleCommandBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _vin = "NEWVIN123456789";
    private string _manufacturer = "New Manufacturer";
    private string _model = "New Model";
    private string _productionYear = "2023";
    private decimal? _engineCapacity = 3.0m;
    private int _mileageValue = 20000;
    private EMileageUnit _mileageUnit = EMileageUnit.Miles;
    private string _licensePlate = "NEW1234";
    private int _horsePower = 250;
    private EFuelType _fuelType = EFuelType.Electric;
    private EBodyType _bodyType = EBodyType.SUV;
    private EVehicleType _vehicleType = EVehicleType.Passenger;

    public UpdateVehicleCommandBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public UpdateVehicleCommandBuilder WithVin(string vin)
    {
        _vin = vin;
        return this;
    }

    public UpdateVehicleCommand Build()
    {
        return new UpdateVehicleCommand(
            _id,
            _vin,
            _manufacturer,
            _model,
            _productionYear,
            _engineCapacity,
            _mileageValue,
            _mileageUnit,
            _licensePlate,
            _horsePower,
            _fuelType,
            _bodyType,
            _vehicleType);
    }
}

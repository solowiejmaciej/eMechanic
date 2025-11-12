namespace eMechanic.Domain.Tests.Builders;

using System;
using Common.Result;
using Domain.Vehicle;
using Domain.Vehicle.Enums;

public class VehicleBuilder
{
    private Guid _userId = Guid.NewGuid();
    private string _vin = "JMZGG128271672202";
    private string _manufacturer = "Mazda";
    private string _model = "6";
    private string _year = "2006";
    private decimal? _capacity = 1.9m;
    private int _mileageValue = 150000;
    private EMileageUnit _mileageUnit = EMileageUnit.Kilometers;
    private string _licensePlate = "PZ1W924";
    private int _horsePower = 240;
    private EFuelType _fuel = EFuelType.Gasoline;
    private EBodyType _body = EBodyType.Sedan;
    private EVehicleType _type = EVehicleType.Passenger;

    public Vehicle Build()
    {
        var result = Vehicle.Create(
            _userId,
            _vin,
            _manufacturer,
            _model,
            _year,
            _capacity,
            _mileageValue,
            _mileageUnit,
            _licensePlate,
            _horsePower,
            _fuel,
            _body,
            _type);

        if (result.HasError())
        {
            throw new InvalidOperationException(result.Error!.Message);
        }

        return result.Value!;
    }

    public Result<Vehicle, Error> BuildResult()
    {
        return Vehicle.Create(
            _userId,
            _vin,
            _manufacturer,
            _model,
            _year,
            _capacity,
            _mileageValue,
            _mileageUnit,
            _licensePlate,
            _horsePower,
            _fuel,
            _body,
            _type);
    }

    public VehicleBuilder WithOwnerId(Guid userId)
    {
        _userId = userId;
        return this;
    }

    public VehicleBuilder WithVin(string vin)
    {
        _vin = vin;
        return this;
    }

    public VehicleBuilder WithManufacturer(string manufacturer)
    {
        _manufacturer = manufacturer;
        return this;
    }

    public VehicleBuilder WithModel(string model)
    {
        _model = model;
        return this;
    }

    public VehicleBuilder WithProductionYear(string year)
    {
        _year = year;
        return this;
    }

    public VehicleBuilder WithEngineCapacity(decimal? capacity)
    {
        _capacity = capacity;
        return this;
    }

    public VehicleBuilder WithMileage(int value, EMileageUnit unit)
    {
        _mileageValue = value;
        _mileageUnit = unit;
        return this;
    }

    public VehicleBuilder WithLicensePlate(string licensePlate)
    {
        _licensePlate = licensePlate;
        return this;
    }

    public VehicleBuilder WithHorsePower(int horsePower)
    {
        _horsePower = horsePower;
        return this;
    }

    public VehicleBuilder WithFuelType(EFuelType fuelType)
    {
        _fuel = fuelType;
        return this;
    }

    public VehicleBuilder WithBodyType(EBodyType bodyType)
    {
        _body = bodyType;
        return this;
    }

    public VehicleBuilder WithVehicleType(EVehicleType vehicleType)
    {
        _type = vehicleType;
        return this;
    }
}

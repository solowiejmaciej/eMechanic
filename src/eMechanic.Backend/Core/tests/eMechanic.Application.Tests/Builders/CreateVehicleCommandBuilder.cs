namespace eMechanic.Application.Tests.Builders;

using eMechanic.Application.Vehicle.Features.Create;
using eMechanic.Domain.Vehicle.Enums;

public class CreateVehicleCommandBuilder
{
    private string _vin = "JMZGG128271672202";
    private string _manufacturer = "Test Manufacturer";
    private string _model = "Test Model";
    private string _productionYear = "2023";
    private decimal? _engineCapacity = 1.6m;
    private int _mileage = 200;
    private EMileageUnit _mileageUnit = EMileageUnit.Miles;
    private string _licensePlate = "PZ1W924";
    private int _horsePower = 124;
    private EFuelType _fuelType = EFuelType.Gasoline;
    private EBodyType _bodyType = EBodyType.Sedan;
    private EVehicleType _vehicleType = EVehicleType.Passenger;

    public CreateVehicleCommand Build()
    {
        return new CreateVehicleCommand(
            _vin,
            _manufacturer,
            _model,
            _productionYear,
            _engineCapacity,
            _mileage,
            _mileageUnit,
            _licensePlate,
            _horsePower,
            _fuelType,
            _bodyType,
            _vehicleType);
    }

    public CreateVehicleCommandBuilder WithVin(string vin)
    {
        _vin = vin;
        return this;
    }

    public CreateVehicleCommandBuilder WithManufacturer(string manufacturer)
    {
        _manufacturer = manufacturer;
        return this;
    }

    public CreateVehicleCommandBuilder WithModel(string model)
    {
        _model = model;
        return this;
    }

    public CreateVehicleCommandBuilder WithProductionYear(string productionYear)
    {
        _productionYear = productionYear;
        return this;
    }

    public CreateVehicleCommandBuilder WithEngineCapacity(decimal? engineCapacity)
    {
        _engineCapacity = engineCapacity;
        return this;
    }

    public CreateVehicleCommandBuilder WithMileage(int mileage)
    {
        _mileage = mileage;
        return this;
    }

    public CreateVehicleCommandBuilder WithMileageUnit(EMileageUnit mileageUnit)
    {
        _mileageUnit = mileageUnit;
        return this;
    }

    public CreateVehicleCommandBuilder WithLicensePlate(string licensePlate)
    {
        _licensePlate = licensePlate;
        return this;
    }

    public CreateVehicleCommandBuilder WithHorsePower(int horsePower)
    {
        _horsePower = horsePower;
        return this;
    }

    public CreateVehicleCommandBuilder WithFuelType(EFuelType fuelType)
    {
        _fuelType = fuelType;
        return this;
    }

    public CreateVehicleCommandBuilder WithBodyType(EBodyType bodyType)
    {
        _bodyType = bodyType;
        return this;
    }

    public CreateVehicleCommandBuilder WithVehicleType(EVehicleType vehicleType)
    {
        _vehicleType = vehicleType;
        return this;
    }
}

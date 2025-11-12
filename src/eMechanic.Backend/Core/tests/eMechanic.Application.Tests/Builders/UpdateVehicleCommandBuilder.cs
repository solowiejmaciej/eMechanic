namespace eMechanic.Application.Tests.Builders;

using eMechanic.Application.Vehicle.Features.Update;
using eMechanic.Domain.Vehicle.Enums;

public class UpdateVehicleCommandBuilder
{
    private Guid _id = Guid.NewGuid();
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

    public UpdateVehicleCommandBuilder WithManufacturer(string manufacturer)
    {
        _manufacturer = manufacturer;
        return this;
    }

    public UpdateVehicleCommandBuilder WithModel(string model)
    {
        _model = model;
        return this;
    }

    public UpdateVehicleCommandBuilder WithProductionYear(string productionYear)
    {
        _productionYear = productionYear;
        return this;
    }

    public UpdateVehicleCommandBuilder WithEngineCapacity(decimal? engineCapacity)
    {
        _engineCapacity = engineCapacity;
        return this;
    }

    public UpdateVehicleCommandBuilder WithMileage(int mileage)
    {
        _mileage = mileage;
        return this;
    }

    public UpdateVehicleCommandBuilder WithMileageUnit(EMileageUnit mileageUnit)
    {
        _mileageUnit = mileageUnit;
        return this;
    }

    public UpdateVehicleCommandBuilder WithLicensePlate(string licensePlate)
    {
        _licensePlate = licensePlate;
        return this;
    }

    public UpdateVehicleCommandBuilder WithHorsePower(int horsePower)
    {
        _horsePower = horsePower;
        return this;
    }

    public UpdateVehicleCommandBuilder WithFuelType(EFuelType fuelType)
    {
        _fuelType = fuelType;
        return this;
    }

    public UpdateVehicleCommandBuilder WithBodyType(EBodyType bodyType)
    {
        _bodyType = bodyType;
        return this;
    }

    public UpdateVehicleCommandBuilder WithVehicleType(EVehicleType vehicleType)
    {
        _vehicleType = vehicleType;
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
            _mileage,
            _mileageUnit,
            _licensePlate,
            _horsePower,
            _fuelType,
            _bodyType,
            _vehicleType);
    }
}

namespace eMechanic.Domain.Vehicle;

using Common.DDD;
using ValueObjects;
using System;
using Common.Result;
using Enums;
using References.User;

public class Vehicle : AggregateRoot, IUserReferenced
{
    public Guid UserId { get; private set; }
    public Vin Vin { get; private set; }
    public Manufacturer Manufacturer { get; private set; }
    public Model Model { get; private set; }
    public ProductionYear ProductionYear{ get; private set; }
    public EngineCapacity? EngineCapacity { get; private set; }
    public Mileage Mileage { get; private set; }
    public LicensePlate LicensePlate { get; private set; }
    public HorsePower HorsePower { get; private set; }
    public EFuelType FuelType { get; private set; }
    public EBodyType BodyType { get; private set; }
    public EVehicleType VehicleType { get; private set; }

    private Vehicle() { }

    private Vehicle(
        Guid userId,
        Vin vin,
        Manufacturer manufacturer,
        Model model,
        ProductionYear productionYear,
        EngineCapacity? engineCapacity,
        Mileage mileage,
        LicensePlate licensePlate,
        HorsePower horsePower,
        EFuelType fuelType,
        EBodyType bodyType,
        EVehicleType vehicleType)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId can't be empty", nameof(userId));
        }

        SetUserId(userId);
        SetVin(vin);
        SetManufacturer(manufacturer);
        SetModel(model);
        SetProductionYear(productionYear);
        SetMileage(mileage);
        SetEngineCapacity(engineCapacity);
        SetLicensePlate(licensePlate);
        SetHorsePower(horsePower);
        SetFuelType(fuelType);
        SetBodyType(bodyType);
        SetVehicleType(vehicleType);

        RaiseDomainEvent(new VehicleCreatedDomainEvent(this));
    }


    public static Result<Vehicle, Error> Create(
        Guid userId,
        string? vinInput,
        string? manufacturerInput,
        string? modelInput,
        string? productionYearInput,
        decimal? engineCapacityInput,
        int? millageInput,
        EMileageUnit unitInput,
        string? licensePlateInput,
        int? horsePowerInput,
        EFuelType fuelType,
        EBodyType bodyType,
        EVehicleType vehicleType)
    {
        var errors = new List<Error>();

        var combinationResult = ValidateTypeCombination(vehicleType, bodyType);
        if (combinationResult.HasError())
        {
            errors.Add(combinationResult.Error!);
        }

        if (userId == Guid.Empty)
        {
            errors.Add(new Error(EErrorCode.ValidationError, "userId cannot be empty."));
        }

        var vinResult = Vin.Create(vinInput);
        if (vinResult.HasError())
        {
            errors.Add(vinResult.Error!);
        }

        var manufacturerResult = Manufacturer.Create(manufacturerInput);
        if (manufacturerResult.HasError())
        {
            errors.Add(manufacturerResult.Error!);
        }

        var modelResult = Model.Create(modelInput);
        if (modelResult.HasError())
        {
            errors.Add(modelResult.Error!);
        }

        var yearResult = ProductionYear.Create(productionYearInput!);
        if (yearResult.HasError())
        {
            errors.Add(yearResult.Error!);
        }

        EngineCapacity? engineCapacity = null;
        if (engineCapacityInput.HasValue)
        {
            var capacityResult = EngineCapacity.Create(engineCapacityInput.Value);
            if (capacityResult.HasError())
            {
                errors.Add(capacityResult.Error!);
            }
            else
            {
                engineCapacity = capacityResult.Value;
            }
        }

        var mileageResult = Mileage.Create(millageInput, unitInput);
        if (mileageResult.HasError())
        {
            errors.Add(mileageResult.Error!);
        }

        var licensePlateResult = LicensePlate.Create(licensePlateInput);
        if (licensePlateResult.HasError())
        {
            errors.Add(licensePlateResult.Error!);
        }

        var horsePowerResult = HorsePower.Create(horsePowerInput);
        if (horsePowerResult.HasError())
        {
            errors.Add(horsePowerResult.Error!);
        }

        if (errors.Count != 0)
        {
            var aggregatedMessage = string.Join("; ", errors.Select(e => e.Message));
            return new Error(EErrorCode.ValidationError, aggregatedMessage);
        }

        try
        {
            var vehicle = new Vehicle(
                userId,
                vinResult.Value!,
                manufacturerResult.Value!,
                modelResult.Value!,
                yearResult.Value!,
                engineCapacity,
                mileageResult.Value!,
                licensePlateResult.Value!,
                horsePowerResult.Value!,
                fuelType,
                bodyType,
                vehicleType);

            return vehicle;
        }
        catch (ArgumentException ex)
        {
            return new Error(EErrorCode.ValidationError, ex.Message);
        }
    }

    public Result<Success, Error> ChangeUserId(Guid newUserId)
    {
        if (newUserId == Guid.Empty)
        {
            return new Error(EErrorCode.ValidationError, "New UserID cant be empty");
        }
        if (UserId == newUserId) return Result.Success;

        var oldUserId = UserId;
        UserId = newUserId;
        RaiseDomainEvent(new VehicleUserIdChangedDomainEvent(this.Id, oldUserId, newUserId));
        return Result.Success;
    }

    public Result<Success, Error> UpdateVin(string? vinInput)
    {
        var vinResult = Vin.Create(vinInput);
        if (vinResult.HasError()) return vinResult.Error!;
        if (Vin == vinResult.Value) return Result.Success;

        var oldVin = Vin;
        Vin = vinResult.Value!;
        RaiseDomainEvent(new VehicleVinChangedDomainEvent(this.Id, oldVin, Vin));
        return Result.Success;
    }

    public Result<Success, Error> UpdateManufacturer(string? manufacturerInput)
    {
        var manufacturerResult = Manufacturer.Create(manufacturerInput);
        if (manufacturerResult.HasError()) return manufacturerResult.Error!;
        if (Manufacturer == manufacturerResult.Value) return Result.Success;

        var oldManufacturer = Manufacturer;
        Manufacturer = manufacturerResult.Value!;
        RaiseDomainEvent(new VehicleManufacturerChangedDomainEvent(this.Id, oldManufacturer, Manufacturer));
        return Result.Success;
    }

    public Result<Success, Error> UpdateModel(string? modelInput)
    {
        var modelResult = Model.Create(modelInput);
        if (modelResult.HasError()) return modelResult.Error!;
        if (Model == modelResult.Value) return Result.Success;

        var oldModel = Model;
        Model = modelResult.Value!;
        RaiseDomainEvent(new VehicleModelChangedDomainEvent(this.Id, oldModel, Model));
        return Result.Success;
    }

    public Result<Success, Error> UpdateProductionYear(string? yearInput)
    {
        var yearResult = ProductionYear.Create(yearInput!);
        if (yearResult.HasError()) return yearResult.Error!;
        if (ProductionYear == yearResult.Value) return Result.Success;

        var oldYear = ProductionYear;
        ProductionYear = yearResult.Value!;
        RaiseDomainEvent(new VehicleProductionYearChangedDomainEvent(this.Id, oldYear, ProductionYear));
        return Result.Success;
    }

    public Result<Success, Error> UpdateEngineCapacity(decimal? newCapacityValue)
    {
        EngineCapacity? newEngineCapacity = null;
        if (newCapacityValue.HasValue)
        {
            var capacityResult = EngineCapacity.Create(newCapacityValue.Value);
            if (capacityResult.HasError()) return capacityResult.Error!;
            newEngineCapacity = capacityResult.Value;
        }

        if (EngineCapacity == newEngineCapacity) return Result.Success;

        var oldCapacity = EngineCapacity;
        EngineCapacity = newEngineCapacity;
        RaiseDomainEvent(new VehicleEngineCapacityChangedDomainEvent(this.Id, oldCapacity, newEngineCapacity));
        return Result.Success;
    }

    public Result<Success, Error> UpdateMileage(int? newMileageValue, EMileageUnit newMileageUnit)
    {
        var newMileageResult = Mileage.Create(newMileageValue, newMileageUnit);

        if (newMileageResult.HasError())
        {
            return newMileageResult.Error!;
        }

        if (Mileage == newMileageResult.Value)
        {
            return Result.Success;
        }

        var oldMileage = Mileage;
        Mileage = newMileageResult.Value!;

        RaiseDomainEvent(new VehicleMileageChangedDomainEvent(Id, oldMileage, Mileage));

        return Result.Success;
    }

    public Result<Success, Error> UpdateLicensePlate(string? licensePlateInput)
    {
        var newLicensePlateResult = LicensePlate.Create(licensePlateInput);
        if (newLicensePlateResult.HasError())
        {
            return newLicensePlateResult.Error!;
        }

        if (LicensePlate == newLicensePlateResult.Value)
        {
            return Result.Success;
        }

        var oldLicensePlate = LicensePlate;
        LicensePlate = newLicensePlateResult.Value!;

        RaiseDomainEvent(new VehicleLicensePlateChangedDomainEvent(Id, oldLicensePlate, LicensePlate));

        return Result.Success;
    }

    public Result<Success, Error> UpdateHorsePower(int? horsePowerInput)
    {
        var newHorsePowerResult = HorsePower.Create(horsePowerInput);
        if (newHorsePowerResult.HasError())
        {
            return newHorsePowerResult.Error!;
        }

        if (HorsePower == newHorsePowerResult.Value)
        {
            return Result.Success;
        }

        var oldHorsePower = HorsePower;
        HorsePower = newHorsePowerResult.Value!;

        RaiseDomainEvent(new VehicleHorsePowerChangedDomainEvent(Id, oldHorsePower, HorsePower));

        return Result.Success;
    }

    public Result<Success, Error> ChangeFuelType(EFuelType newFuelType)
    {
        if (!Enum.IsDefined(newFuelType) || newFuelType == EFuelType.None)
        {
            return new Error(EErrorCode.ValidationError, "Invalid new fuel type provided.");
        }
        if (FuelType == newFuelType) return Result.Success;

        var oldFuelType = FuelType;
        FuelType = newFuelType;
        RaiseDomainEvent(new VehicleFuelTypeChangedDomainEvent(this.Id, oldFuelType, newFuelType));

        return Result.Success;
    }

    public Result<Success, Error> UpdateClassification(EBodyType newBodyType, EVehicleType newVehicleType)
    {
        if (!Enum.IsDefined(newBodyType))
        {
            return new Error(EErrorCode.ValidationError, "Invalid new body type provided.");
        }
        if (!Enum.IsDefined(newVehicleType) || newVehicleType == EVehicleType.None)
        {
            return new Error(EErrorCode.ValidationError, "Invalid new vehicle type provided.");
        }

        var validationResult = ValidateTypeCombination(newVehicleType, newBodyType);
        if (validationResult.HasError())
        {
            return validationResult.Error!;
        }

        if (BodyType != newBodyType)
        {
            var oldBodyType = BodyType;
            BodyType = newBodyType;
            RaiseDomainEvent(new VehicleBodyTypeChangedDomainEvent(this.Id, oldBodyType, newBodyType));
        }

        if (VehicleType != newVehicleType)
        {
            var oldVehicleType = VehicleType;
            VehicleType = newVehicleType;
            RaiseDomainEvent(new VehicleTypeChangedDomainEvent(this.Id, oldVehicleType, newVehicleType));
        }

        return Result.Success;
    }

    private void SetMileage(Mileage mileage) => Mileage = mileage ?? throw new ArgumentNullException(nameof(mileage));

    private void SetEngineCapacity(EngineCapacity? engineCapacity) => EngineCapacity = engineCapacity;

    private void SetProductionYear(ProductionYear productionYear) => ProductionYear = productionYear ?? throw new ArgumentNullException(nameof(productionYear));

    private void SetModel(Model model) => Model = model ?? throw new ArgumentNullException(nameof(model));

    private void SetManufacturer(Manufacturer manufacturer) => Manufacturer = manufacturer ?? throw new ArgumentNullException(nameof(manufacturer));

    private void SetVin(Vin vin) => Vin = vin ?? throw new ArgumentNullException(nameof(vin));

    private void SetLicensePlate(LicensePlate licensePlate) => LicensePlate = licensePlate ?? throw new ArgumentNullException(nameof(licensePlate));

    private void SetHorsePower(HorsePower horsePower) => HorsePower = horsePower ?? throw new ArgumentNullException(nameof(horsePower));


    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(userId));
        }

        UserId = userId;
    }

    private void SetFuelType(EFuelType fuelType)
    {
        if (!Enum.IsDefined(fuelType) || fuelType == EFuelType.None)
        {
            throw new ArgumentException("Invalid fuel type", nameof(fuelType));
        }
        FuelType = fuelType;
    }

    private void SetBodyType(EBodyType bodyType)
    {
        if (!Enum.IsDefined(bodyType))
        {
            throw new ArgumentException("Invalid body type", nameof(bodyType));
        }
        BodyType = bodyType;
    }

    private void SetVehicleType(EVehicleType vehicleType)
    {
        if (!Enum.IsDefined(vehicleType) || vehicleType == EVehicleType.None)
        {
            throw new ArgumentException("Invalid vehicle type", nameof(vehicleType));
        }
        VehicleType = vehicleType;
    }

    private static Result<Success, Error> ValidateTypeCombination(EVehicleType vehicleType, EBodyType bodyType)
    {
        if (vehicleType == EVehicleType.Motorcycle && bodyType != EBodyType.None)
        {
            return new Error(EErrorCode.ValidationError, $"BodyType must be {EBodyType.None} when VehicleType is {EVehicleType.Motorcycle}.");
        }

        if (vehicleType != EVehicleType.Motorcycle && bodyType == EBodyType.None)
        {
            return new Error(EErrorCode.ValidationError, $"BodyType must be specified (cannot be {EBodyType.None}) when VehicleType is not {EVehicleType.Motorcycle}.");
        }

        return Result.Success;
    }
}

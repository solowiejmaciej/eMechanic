namespace eMechanic.Domain.Vehicle;

using Common.DDD;
using ValueObjects;
using System;
using Common.Result;
using DomainEvents;
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
    public EFuelType FuelType { get; private set; }
    public EBodyType BodyType { get; private set; }
    public EVehicleType VehicleType { get; private set; }

    private Vehicle() { }

    private Vehicle(
        Guid userId,
        Vin vin,
        Manufacturer manufacturer,
        Model model,
        ProductionYear productionProductionYear,
        EngineCapacity? engineCapacity,
        EFuelType fuelType,
        EBodyType bodyType,
        EVehicleType vehicleType)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId can't be empty", nameof(userId));
        }

        UserId = userId;

        Vin = vin ?? throw new ArgumentNullException(nameof(vin));
        Manufacturer = manufacturer ?? throw new ArgumentNullException(nameof(manufacturer));
        Model = model ?? throw new ArgumentNullException(nameof(model));
        ProductionYear = productionProductionYear ?? throw new ArgumentNullException(nameof(productionProductionYear));
        EngineCapacity = engineCapacity;

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
        EFuelType fuelType,
        EBodyType bodyType,
        EVehicleType vehicleType)
    {
        var errors = new List<Error>();

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
        SetUpdatedAt();
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
        SetUpdatedAt();
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
        SetUpdatedAt();
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
        SetUpdatedAt();
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
        SetUpdatedAt();
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
        SetUpdatedAt();
        return Result.Success;
    }

    public Result<Success, Error> ChangeBodyType(EBodyType newBodyType)
    {
        if (!Enum.IsDefined(newBodyType) || newBodyType == EBodyType.None)
        {
            return new Error(EErrorCode.ValidationError, "Invalid new body type provided.");
        }
        if (BodyType == newBodyType) return Result.Success;

        var oldBodyType = BodyType;
        BodyType = newBodyType;
        RaiseDomainEvent(new VehicleBodyTypeChangedDomainEvent(this.Id, oldBodyType, newBodyType));
        SetUpdatedAt();
        return Result.Success;
    }

    public Result<Success, Error> ChangeVehicleType(EVehicleType newVehicleType)
    {
        if (!Enum.IsDefined(newVehicleType) || newVehicleType == EVehicleType.None)
        {
            return new Error(EErrorCode.ValidationError, "Invalid new vehicle type provided.");
        }
        if (VehicleType == newVehicleType) return Result.Success;

        var oldVehicleType = VehicleType;
        VehicleType = newVehicleType;
        RaiseDomainEvent(new VehicleTypeChangedDomainEvent(this.Id, oldVehicleType, newVehicleType));
        SetUpdatedAt();
        return Result.Success;
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
        if (!Enum.IsDefined(bodyType) || bodyType == EBodyType.None)
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
}

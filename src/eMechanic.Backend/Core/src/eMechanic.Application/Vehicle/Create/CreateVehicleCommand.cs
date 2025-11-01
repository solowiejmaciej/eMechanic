namespace eMechanic.Application.Vehicle.Create;

using eMechanic.Common.CQRS;
using eMechanic.Domain.Vehicle.Enums;
using FluentValidation;

public sealed record CreateVehicleCommand(
    string Vin,
    string Manufacturer,
    string Model,
    string ProductionYear,
    decimal? EngineCapacity,
    int Mileage,
    EMileageUnit MileageUnit,
    EFuelType FuelType,
    EBodyType BodyType,
    EVehicleType VehicleType) : IResultCommand<Guid>;

public class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleCommandValidator()
    {
        RuleFor(x => x.Vin).NotEmpty().Length(17);
        RuleFor(x => x.Manufacturer).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Model).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ProductionYear).NotEmpty().Length(4);
        RuleFor(x => x.EngineCapacity).GreaterThan(0).When(x => x.EngineCapacity.HasValue);
        RuleFor(x => x.Mileage).NotNull().GreaterThanOrEqualTo(0);
        RuleFor(x => x.MileageUnit).IsInEnum().NotEqual(EMileageUnit.None);
        RuleFor(x => x.FuelType).IsInEnum().NotEqual(EFuelType.None);
        RuleFor(x => x.BodyType).IsInEnum().NotEqual(EBodyType.None);
        RuleFor(x => x.VehicleType).IsInEnum().NotEqual(EVehicleType.None);
    }
}

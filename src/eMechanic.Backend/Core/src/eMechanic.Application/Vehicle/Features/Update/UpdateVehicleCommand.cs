namespace eMechanic.Application.Vehicle.Features.Update;

using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using eMechanic.Domain.Vehicle.Enums;
using FluentValidation;

public sealed record UpdateVehicleCommand(
    Guid Id,
    string Vin,
    string Manufacturer,
    string Model,
    string ProductionYear,
    decimal? EngineCapacity,
    int MillageValue,
    EMileageUnit MillageUnit,
    string LicensePlate,
    int HorsePower,
    EFuelType FuelType,
    EBodyType BodyType,
    EVehicleType VehicleType) : IResultCommand<Success>;

public class UpdateVehicleCommandValidator : AbstractValidator<UpdateVehicleCommand>
{
    public UpdateVehicleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().NotEqual(Guid.Empty);
        RuleFor(x => x.Vin).NotEmpty().Length(17);
        RuleFor(x => x.Manufacturer).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Model).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ProductionYear).NotEmpty().Length(4);
        RuleFor(x => x.EngineCapacity).GreaterThan(0).When(x => x.EngineCapacity.HasValue);
        RuleFor(x => x.MillageValue).GreaterThan(0);
        RuleFor(x => x.MillageUnit).IsInEnum().NotEqual(EMileageUnit.None);
        RuleFor(x => x.FuelType).IsInEnum().NotEqual(EFuelType.None);
        RuleFor(x => x.BodyType)
            .NotEqual(EBodyType.None)
            .When(x => x.VehicleType != EVehicleType.Motorcycle)
            .WithMessage($"BodyType must be specified (cannot be None) when VehicleType is not {EVehicleType.Motorcycle}.");
        RuleFor(x => x.BodyType)
            .Equal(EBodyType.None)
            .When(x => x.VehicleType == EVehicleType.Motorcycle)
            .WithMessage($"BodyType must be None when VehicleType is {EVehicleType.Motorcycle}.");
        RuleFor(x => x.LicensePlate)
            .NotEmpty()
            .MaximumLength(15)
            .Matches("^[a-zA-Z0-9 -]*$");
        RuleFor(x => x.HorsePower)
            .NotEmpty()
            .GreaterThan(0)
            .LessThanOrEqualTo(3000);
    }
}

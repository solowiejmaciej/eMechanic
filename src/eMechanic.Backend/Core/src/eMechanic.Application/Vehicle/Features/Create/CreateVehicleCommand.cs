namespace eMechanic.Application.Vehicle.Features.Create;

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
    string LicensePlate,
    int HorsePower,
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

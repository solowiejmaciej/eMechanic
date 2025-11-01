namespace eMechanic.Domain.Vehicle.ValueObjects;

using Common.Result;
using Enums;

public record Mileage
{
    public int Value { get; }
    public EMileageUnit Unit { get; }
    private const int MIN_VALUE = 0;

    private Mileage(int value, EMileageUnit unit)
    {
        Value = value;
        Unit = unit;
    }

    public static Result<Mileage, Error> Create(int? value, EMileageUnit unit)
    {
        if (!value.HasValue)
        {
            return new Error(EErrorCode.ValidationError, $"{nameof(Mileage)} cannot be null");
        }

        if (value < MIN_VALUE)
        {
            return new Error(EErrorCode.ValidationError, $"{nameof(Mileage)} must be positive integer");
        }

        if (unit == EMileageUnit.None)
        {
            return new Error(EErrorCode.ValidationError, $"{nameof(Mileage)} Unit can't be None");
        }

        return new Mileage(value.Value, unit);
    }

    public static implicit operator int(Mileage model) => model.Value;
    public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
}


namespace eMechanic.Domain.Vehicle.ValueObjects;

using Common.Result;

public record HorsePower
{
    public int Value { get; }
    private const int MAX_REALISTIC_HP = 3000;

    private HorsePower(int value)
    {
        Value = value;
    }

    public static Result<HorsePower, Error> Create(int? value)
    {
        if (value == null)
        {
            return new Error(EErrorCode.ValidationError, "HorsePower can't be null");
        }

        if (value <= 0)
        {
            return new Error(EErrorCode.ValidationError, "HorsePower must be a positive value.");
        }

        if (value > MAX_REALISTIC_HP)
        {
            return new Error(EErrorCode.ValidationError, $"HorsePower value ({value} HP) is unrealistic.");
        }

        return new HorsePower(value.Value);
    }

    public static implicit operator int(HorsePower horsePower) => horsePower.Value;
}

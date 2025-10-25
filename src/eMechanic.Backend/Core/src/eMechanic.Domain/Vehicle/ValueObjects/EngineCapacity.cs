namespace eMechanic.Domain.Vehicle.ValueObjects;
using Common.Result;
public record EngineCapacity
{
    public decimal Value { get; }
    private EngineCapacity(decimal value)
    {
        Value = value;
    }

    public static Result<EngineCapacity, Error> Create(decimal value)
    {
        if (value <= 0)
        {
            return new Error(EErrorCode.ValidationError, "Engine capacity must be a positive value.");
        }

        return new EngineCapacity(value);
    }

    public static implicit operator decimal(EngineCapacity engineCapacity) => engineCapacity.Value;
}

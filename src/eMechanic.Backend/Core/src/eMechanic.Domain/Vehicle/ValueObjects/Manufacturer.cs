namespace eMechanic.Domain.Vehicle.ValueObjects;

using Common.Result;

public record Manufacturer
{
    public string Value { get; }
    private const int MAX_LENGTH = 100;
    private Manufacturer(string value) { Value = value; }

    public static Result<Manufacturer, Error> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new Error(EErrorCode.ValidationError, "Manufacturer name cannot be null or empty.");
        }
        if (value.Length > MAX_LENGTH)
        {
            return new Error(EErrorCode.ValidationError, $"Manufacturer name cannot exceed {MAX_LENGTH} characters.");
        }
        return new Manufacturer(value.Trim());
    }

    public static implicit operator string(Manufacturer manufacturer) => manufacturer.Value;
    public override string ToString() => Value;
}

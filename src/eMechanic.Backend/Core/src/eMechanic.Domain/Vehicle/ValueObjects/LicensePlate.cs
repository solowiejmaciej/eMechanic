namespace eMechanic.Domain.Vehicle.ValueObjects;


using System.Text.RegularExpressions;
using Common.Result;

public record LicensePlate
{
    public string Value { get; }
    private const int MAX_LENGTH = 15;
    private static readonly Regex PlatePattern = new($"^[A-Z0-9 -]+$");

    private LicensePlate(string value)
    {
        Value = value;
    }

    public static Result<LicensePlate, Error> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new Error(EErrorCode.ValidationError, "LicensePlate cannot be null or empty.");
        }

        var normalized = value.Trim().ToUpperInvariant();

        if (normalized.Length > MAX_LENGTH)
        {
            return new Error(EErrorCode.ValidationError, $"LicensePlate cannot exceed {MAX_LENGTH} characters.");
        }

        if (!PlatePattern.IsMatch(normalized))
        {
            return new Error(EErrorCode.ValidationError, "LicensePlate contains invalid characters.");
        }

        return new LicensePlate(normalized);
    }

    public static implicit operator string(LicensePlate licensePlate) => licensePlate.Value;
    public override string ToString() => Value;
}

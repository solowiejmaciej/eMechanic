namespace eMechanic.Domain.Vehicle.ValueObjects;

using System.Text.RegularExpressions;
using Common.Result;

public record Vin
{
    public string Value { get; }
    private const int MAX_LENGTH = 17;
    private static readonly Regex VinPattern = new($"^[A-HJ-NPR-Z0-9]{{{MAX_LENGTH}}}$", RegexOptions.IgnoreCase);

    private Vin(string value)
    {
        Value = value;
    }

    public static Result<Vin, Error> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new Error(EErrorCode.ValidationError, "VIN Cannot be null or empty.");
        }

        var normalizedVin = value.Trim().ToUpperInvariant();

        if (normalizedVin.Length != MAX_LENGTH)
        {
            return new Error(EErrorCode.ValidationError, $"VIN Must be exactly {MAX_LENGTH} characters long.");
        }

        if (!VinPattern.IsMatch(normalizedVin))
        {
             return new Error(EErrorCode.ValidationError, "VIN Contains invalid characters.");
        }


        return new Vin(normalizedVin);
    }

     public static implicit operator string(Vin vin) => vin.Value;

     public override string ToString() => Value;
}

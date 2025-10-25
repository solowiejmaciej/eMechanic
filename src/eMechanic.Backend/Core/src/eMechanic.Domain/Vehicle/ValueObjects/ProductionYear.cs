namespace eMechanic.Domain.Vehicle.ValueObjects;

using Common.Result;

public record ProductionYear
{
    public string Value { get; }
    private const int MIN_PRODUCTION_YEAR = 1886;
    private const int MAX_PRODUCTION_YEAR = 2100;

    private ProductionYear(string value) { Value = value; }

    public static Result<ProductionYear, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new Error(EErrorCode.ValidationError, "Year cannot be null or empty.");
        }

        if (!int.TryParse(value, out int yearInt))
        {
            return new Error(EErrorCode.ValidationError, "Year must be a valid integer.");
        }

        if (yearInt < MIN_PRODUCTION_YEAR || yearInt > MAX_PRODUCTION_YEAR)
        {
            return new Error(EErrorCode.ValidationError, $"Year must be between {MIN_PRODUCTION_YEAR} and {MAX_PRODUCTION_YEAR}.");
        }

        return new ProductionYear(value.Trim());
    }

    public static implicit operator string(ProductionYear model) => model.Value;
    public override string ToString() => Value;
}

namespace eMechanic.Domain.RepairRequest.ValueObjects;

using Common.Result;

public record RepairDescription
{
    public string Value { get; }
    private const int MAX_LENGTH = 2000;
    private const int MIN_LENGTH = 10;

    private RepairDescription(string value) => Value = value;

    public static Result<RepairDescription, Error> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new Error(EErrorCode.ValidationError, "Description cannot be empty.");
        }

        var trimmedValue = value.Trim();

        if (trimmedValue.Length < MIN_LENGTH)
        {
            return new Error(EErrorCode.ValidationError, $"Description must be at least {MIN_LENGTH} characters long.");
        }

        if (trimmedValue.Length > MAX_LENGTH)
        {
            return new Error(EErrorCode.ValidationError, $"Description cannot exceed {MAX_LENGTH} characters.");
        }

        return new RepairDescription(trimmedValue);
    }

    public static implicit operator string(RepairDescription d) => d.Value;
}

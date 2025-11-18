namespace eMechanic.Domain.RepairRequest.ValueObjects;

using Common.Result;

public record RepairDiagnosis
{
    public string Value { get; }
    private const int MAX_LENGTH = 4000;

    private RepairDiagnosis(string value) => Value = value;

    public static Result<RepairDiagnosis, Error> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new Error(EErrorCode.ValidationError, "Diagnosis cannot be empty.");
        }

        var trimmedValue = value.Trim();

        if (trimmedValue.Length > MAX_LENGTH)
        {
            return new Error(EErrorCode.ValidationError, $"Diagnosis cannot exceed {MAX_LENGTH} characters.");
        }

        return new RepairDiagnosis(trimmedValue);
    }

    public static implicit operator string(RepairDiagnosis d) => d.Value;
}

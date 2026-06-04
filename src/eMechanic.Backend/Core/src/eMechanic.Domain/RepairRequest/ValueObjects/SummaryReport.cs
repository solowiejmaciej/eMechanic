namespace eMechanic.Domain.RepairRequest.ValueObjects;

using Common.Result;

public record SummaryReport
{
    public string Value { get; }

    private const int MaxLength = 4000;

    private SummaryReport(string value) => Value = value;

    public static Result<SummaryReport, Error> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new Error(EErrorCode.ValidationError, "Summary report cannot be empty.");
        }

        var trimmed = value.Trim();

        if (trimmed.Length > MaxLength)
        {
            return new Error(EErrorCode.ValidationError, $"Summary report cannot exceed {MaxLength} characters.");
        }

        return new SummaryReport(trimmed);
    }

    public static implicit operator string(SummaryReport report) => report.Value;
}


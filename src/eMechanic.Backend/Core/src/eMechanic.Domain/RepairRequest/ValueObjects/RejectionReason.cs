namespace eMechanic.Domain.RepairRequest.ValueObjects;

using Common.Result;

public record RejectionReason
{
    public string Value { get; }

    private const int MaxLength = 500;

    private RejectionReason(string value) => Value = value;

    public static Result<RejectionReason, Error> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new Error(EErrorCode.ValidationError, "Rejection reason is required.");
        }

        var trimmed = value.Trim();

        if (trimmed.Length > MaxLength)
        {
            return new Error(EErrorCode.ValidationError, $"Rejection reason cannot exceed {MaxLength} characters.");
        }

        return new RejectionReason(trimmed);
    }

    public static implicit operator string(RejectionReason reason) => reason.Value;
}


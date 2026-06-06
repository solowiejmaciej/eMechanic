namespace eMechanic.Domain.Shared.ValueObjects;

using Common.Result;

public record Email
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Result<Email, Error> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new Error(EErrorCode.ValidationError, "Email cannot be empty.");
        }

        var trimmed = value.Trim();

        if (!trimmed.Contains('@') || !trimmed.Contains('.'))
        {
            return new Error(EErrorCode.ValidationError, "Email address is invalid.");
        }

        return new Email(trimmed.ToLowerInvariant());
    }

    public static implicit operator string(Email e) => e.Value;

    public override string ToString() => Value;
}


namespace eMechanic.Domain.Shared.ValueObjects;

using System.Text.RegularExpressions;
using Common.Result;

public record PhoneNumber
{
    private static readonly Regex PhoneRegex =
        new(@"^\+?[\d\s\-\(\)]{7,20}$", RegexOptions.Compiled);

    public string Value { get; }

    private PhoneNumber(string value) => Value = value;

    public static Result<PhoneNumber, Error> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new Error(EErrorCode.ValidationError, "Phone number cannot be empty.");
        }

        var trimmed = value.Trim();

        if (!PhoneRegex.IsMatch(trimmed))
        {
            return new Error(EErrorCode.ValidationError,
                "Phone number is invalid. Use digits, spaces, dashes, parentheses or a leading '+'.");
        }

        return new PhoneNumber(trimmed);
    }

    public static implicit operator string(PhoneNumber p) => p.Value;

    public override string ToString() => Value;
}


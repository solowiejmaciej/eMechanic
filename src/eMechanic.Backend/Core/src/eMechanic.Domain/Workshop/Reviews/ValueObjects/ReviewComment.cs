namespace eMechanic.Domain.Workshop.Reviews.ValueObjects;

using Common.Result;

public sealed record ReviewComment
{
    public const int MAX_LENGTH = 1000;

    public string Value { get; }

    private ReviewComment(string value)
    {
        Value = value;
    }

    public static Result<ReviewComment, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new Error(EErrorCode.ValidationError, "Review comment cannot be empty when provided.");
        }

        if (value.Length > MAX_LENGTH)
        {
            return new Error(EErrorCode.ValidationError, $"Review comment cannot be longer than {MAX_LENGTH} characters.");
        }

        return new ReviewComment(value.Trim());
    }

    public override string ToString() => Value;
}

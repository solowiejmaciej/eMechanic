namespace eMechanic.Domain.Workshop.Reviews.ValueObjects;

using Common.Result;

public readonly record struct ReviewRating
{
    public byte Value { get; }

    private ReviewRating(byte value)
    {
        Value = value;
    }

    public static Result<ReviewRating, Error> Create(byte value)
    {
        if (value is < 1 or > 5)
        {
            return new Error(EErrorCode.ValidationError, "Review rating must be between 1 and 5.");
        }

        return new ReviewRating(value);
    }

    public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
}



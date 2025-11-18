namespace eMechanic.Domain.Shared.ValueObjects;

using Common.Result;

public record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Result<Money, Error> Create(decimal amount, string currency = "PLN")
    {
        if (amount < 0)
        {
            return new Error(EErrorCode.ValidationError, "Money amount cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            return new Error(EErrorCode.ValidationError, "Currency code cannot be empty.");
        }

        if (currency.Length != 3)
        {
            return new Error(EErrorCode.ValidationError, "Currency code must be 3 characters long.");
        }

        return new Money(amount, currency.ToUpperInvariant());
    }

    public static Money Zero(string currency = "PLN") => new(0, currency);

    public override string ToString() => $"{Amount:F2} {Currency}";
}

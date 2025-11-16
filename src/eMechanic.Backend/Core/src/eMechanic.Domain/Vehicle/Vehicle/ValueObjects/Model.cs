namespace eMechanic.Domain.Vehicle.ValueObjects;

using Common.Result;

public record Model
{
    public string Value { get; }
    private const int MAX_LENGTH = 100;

    private Model(string value) { Value = value; }

    public static Result<Model, Error> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new Error(EErrorCode.ValidationError, "Model cannot be null or empty.");
        }
        if (value.Length > MAX_LENGTH)
        {
            return new Error(EErrorCode.ValidationError, $"Model cannot exceed {MAX_LENGTH} characters.");
        }
        return new Model(value.Trim());
    }

    public static implicit operator string(Model model) => model.Value;
    public override string ToString() => Value;
}

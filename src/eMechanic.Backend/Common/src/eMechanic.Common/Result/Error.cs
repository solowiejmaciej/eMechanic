namespace eMechanic.Common.Result;

using System.Collections.ObjectModel;
using Fields;

public sealed class Error
{
    public EErrorCode Code { get; }
    public string? Message { get; }
    public IReadOnlyDictionary<string, string[]>? ValidationErrors { get; }

    public Error(EErrorCode code)
    {
        Code = code;
        Message = code.ToString();
    }

    public Error(EErrorCode code, string message)
    {
        Code = code;
        Message = message;
        ValidationErrors = Array.Empty<KeyValuePair<string, string[]>>()
            .ToDictionary(kv => kv.Key, kv => kv.Value);
    }


    public static Error Validation(EField field, string message)
    {
        var errors = new Dictionary<string, string[]>
        {
            { field.ToString(), new[] { message } }
        };

        return new Error(EErrorCode.ValidationError, new ReadOnlyDictionary<string, string[]>(errors));
    }

    public Error(EErrorCode code, IReadOnlyDictionary<string, string[]> validationErrors)
    {
        Code = code;
        Message = Code.ToString();
        ValidationErrors = validationErrors;
    }

    public override string ToString() => Message ?? Code.ToString();

}

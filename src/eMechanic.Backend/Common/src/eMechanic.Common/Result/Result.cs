namespace eMechanic.Common.Result;

using System.Collections.Generic;
using System.Text.Json.Serialization;

public class Result<TValue, TError>
{
    public TValue? Value { get; private init; }
    public TError? Error { get; private init; }
    public bool IsSuccess { get; }

    [JsonConstructor]
    internal Result(TValue value)
    {
        IsSuccess = true;
        Value = value;
        Error = default;
    }

    internal Result(TError error)
    {
        IsSuccess = false;
        Value = default;
        Error = error;
    }

    public static implicit operator Result<TValue, TError>(TValue value) => new(value);

    public static implicit operator Result<TValue, TError>(TError error) => new(error);

    public static Result<IEnumerable<T>, TError> FromEnumerable<T>(IEnumerable<T> value) => new(value);

    public bool HasError() => !IsSuccess;
}


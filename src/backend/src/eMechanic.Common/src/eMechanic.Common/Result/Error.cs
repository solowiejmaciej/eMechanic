namespace eMechanic.Common.Result;

public sealed class Error
{
    public EErrorCode Code { get; }
    public string? Message { get; }

    public Error(EErrorCode code)
    {
        Code = code;
        Message = code.ToString();
    }

    public Error(EErrorCode code, string message)
    {
        Code = code;
        Message = message;
    }

    public override string ToString() => Message ?? Code.ToString();

}

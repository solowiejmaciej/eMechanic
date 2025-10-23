namespace eMechanic.Infrastructure.Exceptions;

using Common.Result;

public class RegistrationException : Exception
{
    public Error Error { get; }

    public RegistrationException(Error error) : base(error.Message)
    {
        Error = error;
    }

    public RegistrationException()
    {
    }
    public RegistrationException(string message) : base(message)
    {
    }
    public RegistrationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

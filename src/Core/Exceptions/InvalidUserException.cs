namespace Goals.Core.Exceptions;

public class InvalidUserException : Exception
{
    public InvalidUserException() : base("User is invalid") {}

    public InvalidUserException(string message) : base(message) {}
}

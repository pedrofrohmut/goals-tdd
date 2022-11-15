namespace Goals.Core.Exceptions;

public class EmailAlreadyTakenException : Exception
{
    public EmailAlreadyTakenException() : base("User e-mail is already registered and must be unique") {}

    public EmailAlreadyTakenException(string message) : base(message) {}
}

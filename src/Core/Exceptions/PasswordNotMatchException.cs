namespace Goals.Core.Exceptions;

public class PasswordNotMatchException : Exception
{
    public PasswordNotMatchException() : base("Password is not a match to the password hash") {}

    public PasswordNotMatchException(string message) : base(message) {}
}

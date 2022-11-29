namespace Goals.Core.Exceptions;

public class InvalidGoalException : Exception
{
    public InvalidGoalException() : base("Goal is invalid") {}

    public InvalidGoalException(string message) : base(message) {}
}

using Goals.Core.Exceptions;

namespace Goals.Core.Validators;

public class UserValidator : IUserValidator
{
    public void ValidateName(string name)
    {
        if (String.IsNullOrWhiteSpace(name)) {
            throw new InvalidUserException("User is required and cannot be empty");
        }
    }
}

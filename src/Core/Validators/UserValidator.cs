using System.ComponentModel.DataAnnotations;
using Goals.Core.Exceptions;

namespace Goals.Core.Validators;

public class UserValidator : IUserValidator
{
    public void ValidateId(string userId)
    {
        if (String.IsNullOrWhiteSpace(userId)) {
            throw new InvalidUserException("User id is required and cannot be empty");
        }
        var isValid = Guid.TryParse(userId, out var _result);
        if (! isValid) {
            throw new InvalidUserException("User id is not a valid GUID (globally unique identifier)");
        }
    }

    public void ValidateName(string name)
    {
        if (String.IsNullOrWhiteSpace(name)) {
            throw new InvalidUserException("User name is required and cannot be empty");
        }
        if (name.Length < 3 || name.Length > 120) {
            throw new InvalidUserException("User name must between 3 and 120 characters long");
        }
    }

    public void ValidateEmail(string email)
    {
        if (String.IsNullOrWhiteSpace(email)) {
            throw new InvalidUserException("User e-mail is required and cannot be empty");
        }
        bool isValid = new EmailAddressAttribute().IsValid(email);
        if (! isValid) {
            throw new InvalidUserException("User e-mail format is invalid");
        }
    }

    public void ValidatePassword(string password)
    {
        if (String.IsNullOrWhiteSpace(password)) {
            throw new InvalidUserException("User password is required and cannot be empty");
        }
        if (password.Length < 3 || password.Length > 32) {
            throw new InvalidUserException("User password must be between 3 and 32 characters");
        }
    }
}

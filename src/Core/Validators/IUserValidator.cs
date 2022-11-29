namespace Goals.Core.Validators;

public interface IUserValidator
{
    void ValidateId(string userId);
    void ValidateName(string name);
    void ValidateEmail(string email);
    void ValidatePassword(string password);
}

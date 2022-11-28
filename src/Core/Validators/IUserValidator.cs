namespace Goals.Core.Validators;

public interface IUserValidator
{
    public void ValidateId(string userId);
    public void ValidateName(string name);
    public void ValidateEmail(string email);
    public void ValidatePassword(string password);
}

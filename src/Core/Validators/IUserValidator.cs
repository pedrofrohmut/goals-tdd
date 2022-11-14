namespace Goals.Core.Validators;

public interface IUserValidator
{
    public void ValidateName(string name);
    public void ValidateEmail(string email);
    public void ValidatePassword(string password);
}

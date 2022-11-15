namespace Goals.Core.Services;

public interface IPasswordService
{
    Task<string> HashPassword(string password);
}

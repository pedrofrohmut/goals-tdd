namespace Goals.Core.Services;

public interface IPasswordService
{
    Task<string> HashPassword(string password);
    Task<bool> MatchPasswordAndHash(string password, string hash);
}

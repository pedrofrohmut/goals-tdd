namespace Goals.Core.Services;

public class PasswordService : IPasswordService
{
    public Task<string> HashPassword(string password)
    {
        throw new NotImplementedException();
    }

    public Task<bool> MatchPasswordAndHash(string password, string hash)
    {
        throw new NotImplementedException();
    }
}

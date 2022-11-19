namespace Goals.Core.Services;

public interface ITokenService
{
    Task<string> CreateToken(string userId);
}

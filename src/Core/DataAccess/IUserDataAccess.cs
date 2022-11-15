using Goals.Core.Dtos;

namespace Goals.Core.DataAccess;

public interface IUserDataAccess
{
    Task Create(CreateUserDto newUser, string passwordHash);
    Task<UserDbDto?> FindUserByEmail(string email);
}

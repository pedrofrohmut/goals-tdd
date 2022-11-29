using Goals.Core.Dtos;

namespace Goals.Core.DataAccess;

public interface IUserDataAccess
{
    Task Create(CreateUserDto newUser, string passwordHash);
    Task<UserDbDto?> FindByEmail(string email);
    Task<UserDbDto?> FindById(string userId);
}

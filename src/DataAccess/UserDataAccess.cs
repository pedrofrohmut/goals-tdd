using Goals.Core.DataAccess;
using Goals.Core.Dtos;

namespace Goals.DataAccess;

public class UserDataAccess : IUserDataAccess
{
    public Task Create(CreateUserDto newUser, string passwordHash)
    {
        throw new NotImplementedException();
    }

    public Task<UserDbDto?> FindByEmail(string email)
    {
        throw new NotImplementedException();
    }
}

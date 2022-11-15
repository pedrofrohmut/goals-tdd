using Goals.Core.Dtos;

namespace Goals.Core.DataAccess;

public interface IUserDataAccess
{
    Task<UserDbDto?> FindUserByEmail(string email);
}

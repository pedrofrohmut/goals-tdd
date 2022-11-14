using Goals.Core.Dtos;

namespace Goals.Core.UseCases.Users;

public interface ISignUpUserUseCase
{
    Task Execute(CreateUserDto newUser);
}

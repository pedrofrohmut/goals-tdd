using Goals.Core.Dtos;

namespace Goals.Core.UseCases.Users;

public interface ISignInUserUseCase
{
    Task<SignedUserDto> Execute(SignInCredentialsDto credentials);
}

using Goals.Core.Dtos;
using Goals.Core.Validators;

namespace Goals.Core.UseCases.Users;

public class SignUpUserUseCase : ISignUpUserUseCase
{
    private readonly IUserValidator userValidator;

    public SignUpUserUseCase(IUserValidator userValidator)
    {
        this.userValidator = userValidator;
    }

    public async Task Execute(CreateUserDto newUser)
    {
        this.ValidateUser(newUser);
    }

    private void ValidateUser(CreateUserDto newUser)
    {
        this.userValidator.ValidateName(newUser.Name);
    }
}

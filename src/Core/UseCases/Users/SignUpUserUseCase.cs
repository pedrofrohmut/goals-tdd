using Goals.Core.DataAccess;
using Goals.Core.Dtos;
using Goals.Core.Exceptions;
using Goals.Core.Validators;

namespace Goals.Core.UseCases.Users;

public class SignUpUserUseCase : ISignUpUserUseCase
{
    private readonly IUserValidator userValidator;
    private readonly IUserDataAccess userDataAccess;

    public SignUpUserUseCase(IUserValidator userValidator, IUserDataAccess userDataAccess)
    {
        this.userValidator = userValidator;
        this.userDataAccess = userDataAccess;
    }

    public async Task Execute(CreateUserDto newUser)
    {
        this.ValidateUser(newUser);
        await this.CheckEmailIsAvailable(newUser.Email);
    }

    private void ValidateUser(CreateUserDto newUser)
    {
        this.userValidator.ValidateName(newUser.Name);
        this.userValidator.ValidateEmail(newUser.Email);
        this.userValidator.ValidatePassword(newUser.Password);
    }

    private async Task CheckEmailIsAvailable(string email)
    {
        var user = await this.userDataAccess.FindUserByEmail(email);
        if (user != null) {
            throw new EmailAlreadyTakenException();
        }
    }
}

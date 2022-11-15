using Goals.Core.DataAccess;
using Goals.Core.Dtos;
using Goals.Core.Exceptions;
using Goals.Core.Services;
using Goals.Core.Validators;

namespace Goals.Core.UseCases.Users;

public class SignUpUserUseCase : ISignUpUserUseCase
{
    private readonly IUserValidator userValidator;
    private readonly IUserDataAccess userDataAccess;
    private readonly IPasswordService passwordService;

    public SignUpUserUseCase(IUserValidator userValidator,
                             IUserDataAccess userDataAccess,
                             IPasswordService passwordService)
    {
        this.userValidator = userValidator;
        this.userDataAccess = userDataAccess;
        this.passwordService = passwordService;
    }

    public async Task Execute(CreateUserDto newUser)
    {
        this.ValidateUser(newUser);
        await this.CheckEmailIsAvailable(newUser.Email);
        var hash = await this.CreatePasswordHash(newUser.Password);
        await this.CreateUser(newUser, hash);
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

    private Task<string> CreatePasswordHash(string password) =>
        this.passwordService.HashPassword(password);

    private Task CreateUser(CreateUserDto newUser, string hash) =>
        this.userDataAccess.Create(newUser, hash);
}

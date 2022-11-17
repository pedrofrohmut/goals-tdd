using Goals.Core.DataAccess;
using Goals.Core.Dtos;
using Goals.Core.Exceptions;
using Goals.Core.Services;
using Goals.Core.Validators;

namespace Goals.Core.UseCases.Users;

public class SignInUserUseCase : ISignInUserUseCase
{
    private readonly IUserValidator userValidator;
    private readonly IUserDataAccess userDataAccess;
    private readonly IPasswordService passwordService;

    public SignInUserUseCase(IUserValidator userValidator,
                             IUserDataAccess userDataAccess,
                             IPasswordService passwordService)
    {
        this.userValidator = userValidator;
        this.userDataAccess = userDataAccess;
        this.passwordService = passwordService;
    }

    public async Task<SignedUserDto> Execute(SignInCredentialsDto credentials)
    {
        this.ValidateCredentials(credentials);
        var user = await this.FindUser(credentials.Email);
        await this.CheckPasswordMatch(credentials.Password, user.PasswordHash);
        return null;
    }

    private void ValidateCredentials(SignInCredentialsDto credentials)
    {
        this.userValidator.ValidateEmail(credentials.Email);
        this.userValidator.ValidatePassword(credentials.Password);
    }

    private async Task<UserDbDto> FindUser(string email)
    {
        var user = await this.userDataAccess.FindByEmail(email);
        if (user == null) {
            throw new UserNotFoundException("User not found with the e-mail passed");
        }
        return user;
    }

    private async Task CheckPasswordMatch(string password, string hash)
    {
        var isMatch = await this.passwordService.MatchPasswordAndHash(password, hash);
        if (! isMatch) {
            throw new PasswordNotMatchException();
        }
    }
}

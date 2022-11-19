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
    private readonly ITokenService tokenService;

    public SignInUserUseCase(IUserValidator userValidator,
                             IUserDataAccess userDataAccess,
                             IPasswordService passwordService,
                             ITokenService tokenService)
    {
        this.userValidator = userValidator;
        this.userDataAccess = userDataAccess;
        this.passwordService = passwordService;
        this.tokenService = tokenService;
    }

    public async Task<SignedUserDto> Execute(SignInCredentialsDto credentials)
    {
        this.ValidateCredentials(credentials);
        var user = await this.FindUser(credentials.Email);
        await this.CheckPasswordMatch(credentials.Password, user.PasswordHash);
        var token = await this.CreateToken(user.Id);
        return new SignedUserDto() {
            Id = user.Id.ToString(),
            Name = user.Name,
            Email = user.Email,
            Token = token
        };
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

    private Task<string> CreateToken(Guid userId) => this.tokenService.CreateToken(userId.ToString());
}

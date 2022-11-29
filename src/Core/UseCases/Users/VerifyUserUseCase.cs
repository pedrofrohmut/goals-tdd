using Goals.Core.DataAccess;
using Goals.Core.Exceptions;
using Goals.Core.Validators;

namespace Goals.Core.UseCases.Users;

public class VerifyUserUseCase : IVerifyUserUseCase
{
    private readonly IUserValidator userValidator;
    private readonly IUserDataAccess userDataAccess;

    public VerifyUserUseCase(IUserValidator userValidator, IUserDataAccess userDataAccess)
    {
        this.userValidator = userValidator;
        this.userDataAccess = userDataAccess;
    }

    public async Task Execute(string authUserId)
    {
        this.ValidateAuthUserId(authUserId);
        await this.CheckUserExists(authUserId);
    }

    private void ValidateAuthUserId(string authUserId)
    {
        this.userValidator.ValidateId(authUserId);
    }

    private async Task CheckUserExists(string authUserId)
    {
        var user = await this.userDataAccess.FindById(authUserId);
        if (user == null) {
            throw new UserNotFoundException("User Not Found by authUserId");
        }
    }
}

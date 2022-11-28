namespace Goals.Core.UseCases.Users;

public interface IVerifyUserUseCase
{
    Task Execute(string authUserId);
}

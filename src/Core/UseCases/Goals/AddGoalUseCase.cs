using Goals.Core.DataAccess;
using Goals.Core.Dtos;
using Goals.Core.Exceptions;
using Goals.Core.Validators;

namespace Goals.Core.UseCases.Goals;

public class AddGoalUseCase : IAddGoalUseCase
{
    private readonly IGoalValidator goalValidator;
    private readonly IUserDataAccess userDataAccess;
    private readonly IGoalDataAccess goalDataAccess;

    public AddGoalUseCase(IGoalValidator goalValidator,
                          IUserDataAccess userDataAccess,
                          IGoalDataAccess goalDataAccess)
    {
        this.goalValidator = goalValidator;
        this.userDataAccess = userDataAccess;
        this.goalDataAccess = goalDataAccess;;
    }

    public async Task Execute(CreateGoalDto newGoal, string authUserId)
    {
        this.ValidateNewGoal(newGoal);
        await this.CheckUserExists(authUserId);
        await this.CreateGoal(newGoal, authUserId);
    }

    private void ValidateNewGoal(CreateGoalDto newGoal)
    {
        this.goalValidator.ValidateText(newGoal.Text);
    }

    private async Task CheckUserExists(string authUserId)
    {
        var user = await this.userDataAccess.FindById(authUserId);
        if (user == null) {
            throw new UserNotFoundException();
        }
    }

    private Task CreateGoal(CreateGoalDto newGoal, string authUserId)
    {
        return this.goalDataAccess.Create(newGoal, authUserId);
    }
}

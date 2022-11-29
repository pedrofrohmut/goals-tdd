using Goals.Core.Dtos;

namespace Goals.Core.UseCases.Goals;

public interface IAddGoalUseCase
{
    Task Execute(CreateGoalDto newGoal, string authUserId);
}

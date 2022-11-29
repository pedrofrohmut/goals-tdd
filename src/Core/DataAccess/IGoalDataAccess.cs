using Goals.Core.Dtos;

namespace Goals.Core.DataAccess;

public interface IGoalDataAccess
{
    Task Create(CreateGoalDto newGoal, string userId);
}

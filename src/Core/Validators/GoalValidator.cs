using Goals.Core.Exceptions;

namespace Goals.Core.Validators;

public class GoalValidator : IGoalValidator
{
    public void ValidateId(string goalId)
    {
        if (String.IsNullOrWhiteSpace(goalId)) {
            throw new InvalidGoalException("Goal id is required and cannot be empty");
        }
        var isValid = Guid.TryParse(goalId, out var _result);
        if (! isValid) {
            throw new InvalidGoalException("Goal id is not a valid GUID (Globally Unique IDentifier)");
        }
    }

    public void ValidateText(string text)
    {
        if (String.IsNullOrWhiteSpace(text)) {
            throw new InvalidGoalException("Goal text is required and cannot be empty");
        }
        if (text.Length < 3 || text.Length > 120) {
            throw new InvalidGoalException("Goal text should be between 3 and 120 characters");
        }
    }
}

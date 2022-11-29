using Goals.Core.Exceptions;

namespace Goals.Core.Validators;

public class GoalValidator : IGoalValidator
{
    public void ValidateText(string text)
    {
        if (String.IsNullOrWhiteSpace(text)) {
            throw new InvalidGoalException("Goal text is required and cannot be empty");
        }
    }
}

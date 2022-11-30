namespace Goals.Core.Validators;

public interface IGoalValidator
{
    void ValidateId(string goalId);
    void ValidateText(string text);
}

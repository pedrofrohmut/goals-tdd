using System;
using FluentAssertions;
using Goals.Core.Exceptions;
using Goals.Core.Validators;
using Xunit;

namespace Goals.Tests.Core.Validators;

// Validate
// Id not empty
// Id is valid Guid
// Text is not empty
// Text is between 3 and 120 characters
public class GoalValidatorTest : IDisposable
{
    private readonly IGoalValidator goalValidator;

    // Setup
    public GoalValidatorTest()
    {
        this.goalValidator = new GoalValidator();
    }

    // Clean Up
    public void Dispose() { }

    [Fact]
    void EmptyId_Throws()
    {
        // Given
        var emptyId = "";
        // When
        var result = () => this.goalValidator.ValidateId(emptyId);
        // Then
        result.Should().Throw<InvalidGoalException>()
            .WithMessage("Goal id is required and cannot be empty");
    }

    [Fact]
    void InvalidGuidId_Throws()
    {
        // Given
        var invalidGuidId = "INVALID_GUID";
        // When
        var result = () => this.goalValidator.ValidateId(invalidGuidId);
        // Then
        result.Should().Throw<InvalidGoalException>()
            .WithMessage("Goal id is not a valid GUID (Globally Unique IDentifier)");
    }

    [Fact]
    void EmptyText_Throws()
    {
        // Given
        var emptyText = "";
        // When
        var result = () => this.goalValidator.ValidateText(emptyText);
        // Then
        result.Should().Throw<InvalidGoalException>()
            .WithMessage("Goal text is required and cannot be empty");
    }

    [Fact]
    void ShortText_Throws()
    {
        // Given
        var shortText = "00";
        // When
        var result = () => this.goalValidator.ValidateText(shortText);
        // Then
        result.Should().Throw<InvalidGoalException>()
            .WithMessage("Goal text should be between 3 and 120 characters");
    }

    [Fact]
    void LongText_Throws()
    {
        // Given
        var longText = "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
        // When
        var result = () => this.goalValidator.ValidateText(longText);
        // Then
        result.Should().Throw<InvalidGoalException>()
            .WithMessage("Goal text should be between 3 and 120 characters");
    }
}

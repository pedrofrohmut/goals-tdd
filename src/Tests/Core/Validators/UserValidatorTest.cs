using System;
using FluentAssertions;
using Goals.Core.Exceptions;
using Goals.Core.Validators;
using Xunit;

namespace Goals.Tests.Core.Validators;

// Validate
// Id not empty
// Id valid Guid
// Name not empty
// Name length between 3 and 120
// Email not empty
// Email valid format
// Password not empty
// Password length 3 and 32
public class UserValidatorTest : IDisposable
{
    private readonly IUserValidator userValidator;

    // Setup
    public UserValidatorTest()
    {
        this.userValidator = new UserValidator();
    }

    // Clean Up
    public void Dispose() {}

    [Fact]
    void EmptyId_Throws()
    {
        // Given
        var emptyId = "";
        // When
        var result = () => this.userValidator.ValidateId(emptyId);
        // Then
        result.Should().Throw<InvalidUserException>()
            .WithMessage("User id is required and cannot be empty");
    }

    [Fact]
    void InvalidId_Throws()
    {
        // Given
        var invalidId = "INVALID_GUID";
        // When
        var result = () => this.userValidator.ValidateId(invalidId);
        // Then
        result.Should().Throw<InvalidUserException>()
            .WithMessage("User id is not a valid GUID (Globally Unique IDentifier)");
    }

    [Fact]
    void EmptyName_Throws()
    {
        // Given
        var emptyName = "";
        // When
        var result = () => this.userValidator.ValidateName(emptyName);
        // Then
        result.Should().Throw<InvalidUserException>()
            .WithMessage("User name is required and cannot be empty");
    }

    [Fact]
    void ShortLengthName_Throws()
    {
        // Given
        var shortName = "jo";
        // When
        var result = () => this.userValidator.ValidateName(shortName);
        // Then
        result.Should().Throw<InvalidUserException>()
            .WithMessage("User name must between 3 and 120 characters long");
    }

    [Fact]
    void LongLengthName_Throws()
    {
        // Given
        var longName = "john000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
        // When
        var result = () => this.userValidator.ValidateName(longName);
        // Then
        result.Should().Throw<InvalidUserException>()
            .WithMessage("User name must between 3 and 120 characters long");
    }

    [Fact]
    void EmptyEmail_Throws()
    {
        // Given
        var emptyEmail = "";
        // When
        var result = () => this.userValidator.ValidateEmail(emptyEmail);
        // Then
        result.Should().Throw<InvalidUserException>()
            .WithMessage("User e-mail is required and cannot be empty");
    }

    [Fact]
    void InvalidEmail_Throws()
    {
        // Given
        var invalidEmail = "johndoe";
        // When
        var result = () => this.userValidator.ValidateEmail(invalidEmail);
        // Then
        result.Should().Throw<InvalidUserException>()
            .WithMessage("User e-mail format is invalid");
    }

    [Fact]
    void EmptyPassword_Throws()
    {
        // Given
        var emptyPassword = "";
        // When
        var result = () => this.userValidator.ValidatePassword(emptyPassword);
        // Then
        result.Should().Throw<InvalidUserException>()
            .WithMessage("User password is required and cannot be empty");
    }

    [Fact]
    void ShortPassword_Throws()
    {
        // Given
        var shortPassword = "00";
        // When
        var result = () => this.userValidator.ValidatePassword(shortPassword);
        // Then
        result.Should().Throw<InvalidUserException>()
            .WithMessage("User password must be between 3 and 32 characters");
    }

    [Fact]
    void LongPassword_Throws()
    {
        // Given
        var longPassword = "000000000000000000000000000000000";
        // When
        var result = () => this.userValidator.ValidatePassword(longPassword);
        // Then
        result.Should().Throw<InvalidUserException>()
            .WithMessage("User password must be between 3 and 32 characters");
    }
}

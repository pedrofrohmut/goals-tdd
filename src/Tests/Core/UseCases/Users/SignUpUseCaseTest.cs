using System.Threading.Tasks;
using Goals.Core.Dtos;
using Xunit;
using FluentAssertions;

using Goals.Core.UseCases.Users;
using Goals.Core.Validators;
using Goals.Core.Exceptions;

namespace Goals.Tests.Core.UseCases.Users;

public class SignUpUseCaseTest
{
    [Fact]
    async Task EmptyName_ThrowsInvalidUserException()
    {
        var userValidator = new UserValidator();
        var useCase = new SignUpUserUseCase(userValidator);
        // Given
        var newUser = new CreateUserDto() { Name = "", Email = "john@doe.com", Password = "123" };
        // When
        var result = useCase.Invoking(useCase => useCase.Execute(newUser));
        // Then
        await result.Should().ThrowAsync<InvalidUserException>()
            .WithMessage("User name is required and cannot be empty");
    }

    [Fact]
    async Task ShortLengthName_ThrowsInvalidUserException()
    {
        var userValidator = new UserValidator();
        var useCase = new SignUpUserUseCase(userValidator);
        // Given
        var shortName = "jo";
        var newUser = new CreateUserDto() { Name = shortName, Email = "john@doe.com", Password = "123" };
        // When
        var result = useCase.Invoking(useCase => useCase.Execute(newUser));
        // Then
        await result.Should().ThrowAsync<InvalidUserException>()
            .WithMessage("User name must between 3 and 120 characters long");
    }

    [Fact]
    async Task LongLengthName_ThrowsInvalidUserException()
    {
        var userValidator = new UserValidator();
        var useCase = new SignUpUserUseCase(userValidator);
        // Given
        var longName = "john000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
        var newUser = new CreateUserDto() { Name = longName, Email = "john@doe.com", Password = "123" };
        // When
        var result = useCase.Invoking(useCase => useCase.Execute(newUser));
        // Then
        await result.Should().ThrowAsync<InvalidUserException>()
            .WithMessage("User name must between 3 and 120 characters long");
    }

    [Fact]
    async Task EmptyEmail_ThrowsInvalidUserException()
    {
        var userValidator = new UserValidator();
        var useCase = new SignUpUserUseCase(userValidator);
        // Given
        var emptyEmail = "";
        var newUser = new CreateUserDto() { Name = "John Doe", Email = emptyEmail, Password = "123" };
        // When
        var result = useCase.Invoking(useCase => useCase.Execute(newUser));
        // Then
        await result.Should().ThrowAsync<InvalidUserException>()
            .WithMessage("User e-mail is required and cannot be empty");
    }

    [Fact]
    async Task InvalidEmail_ThrowsInvalidUserException()
    {
        var userValidator = new UserValidator();
        var useCase = new SignUpUserUseCase(userValidator);
        // Given
        var invalidEmail = "johndoe";
        var newUser = new CreateUserDto() { Name = "John Doe", Email = invalidEmail, Password = "123" };
        // When
        var result = useCase.Invoking(useCase => useCase.Execute(newUser));
        // Then
        await result.Should().ThrowAsync<InvalidUserException>()
            .WithMessage("User e-mail format is invalid");
    }

    [Fact]
    async Task EmptyPassword_ThrowsInvalidUserException()
    {
        var userValidator = new UserValidator();
        var useCase = new SignUpUserUseCase(userValidator);
        // Given
        var emptyPassword = "";
        var newUser = new CreateUserDto() { Name = "John Doe",
                                            Email = "john@doe.com",
                                            Password = emptyPassword };
        // When
        var result = useCase.Invoking(useCase => useCase.Execute(newUser));
        // Then
        await result.Should().ThrowAsync<InvalidUserException>()
            .WithMessage("User password is required and cannot be empty");
    }

    [Fact]
    async Task ShortPassword_ThrowsInvalidUserException()
    {
        var userValidator = new UserValidator();
        var useCase = new SignUpUserUseCase(userValidator);
        // Given
        var shortPassword = "00";
        var newUser = new CreateUserDto() { Name = "John Doe",
                                            Email = "john@doe.com",
                                            Password = shortPassword };
        // When
        var result = useCase.Invoking(useCase => useCase.Execute(newUser));
        // Then
        await result.Should().ThrowAsync<InvalidUserException>()
            .WithMessage("User password must be between 3 and 32 characters");
    }

    [Fact]
    async Task LongPassword_ThrowsInvalidUserException()
    {
        var userValidator = new UserValidator();
        var useCase = new SignUpUserUseCase(userValidator);
        // Given
        var longPassword = "000000000000000000000000000000000";
        var newUser = new CreateUserDto() { Name = "John Doe",
                                            Email = "john@doe.com",
                                            Password = longPassword };
        // When
        var result = useCase.Invoking(useCase => useCase.Execute(newUser));
        // Then
        await result.Should().ThrowAsync<InvalidUserException>()
            .WithMessage("User password must be between 3 and 32 characters");
    }
}

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Goals.Core.DataAccess;
using Goals.Core.Dtos;
using Goals.Core.Exceptions;
using Goals.Core.Services;
using Goals.Core.UseCases.Users;
using Goals.Core.Validators;
using Moq;
using Xunit;

namespace Goals.Tests.Core.UseCases.Users;

public class SignInUserUseCaseTest : IDisposable
{
    private readonly Mock<IUserDataAccess> userDAMock;
    private readonly Mock<IPasswordService> passServMock;

    // Setup
    public SignInUserUseCaseTest()
    {
        this.userDAMock = new Mock<IUserDataAccess>();
        this.passServMock = new Mock<IPasswordService>();
    }

    // Clean Up
    public void Dispose() {}

    [Fact]
    async Task EmptyEmail_ThrowsInvalidUserException()
    {
        var useCase = new SignInUserUseCase(new UserValidator(), this.userDAMock.Object, new PasswordService());
        // Given
        var emptyEmail = "";
        var credentials = new SignInCredentialsDto() { Email = emptyEmail, Password = "1234" };
        // When
        var result = useCase.Invoking(useCase => useCase.Execute(credentials));
        // Then
        await result.Should().ThrowAsync<InvalidUserException>()
            .WithMessage("User e-mail is required and cannot be empty");
    }

    [Fact]
    async Task InvalidEmail_ThrowsInvalidUserException()
    {
        var useCase = new SignInUserUseCase(new UserValidator(), this.userDAMock.Object, new PasswordService());
        // Given
        var invalidEmail = "johndoe";
        var newUser = new SignInCredentialsDto() { Email = invalidEmail, Password = "123" };
        // When
        var result = useCase.Invoking(useCase => useCase.Execute(newUser));
        // Then
        await result.Should().ThrowAsync<InvalidUserException>()
            .WithMessage("User e-mail format is invalid");
    }

    [Fact]
    async Task EmptyPassword_ThrowsInvalidUserException()
    {
        var useCase = new SignInUserUseCase(new UserValidator(), this.userDAMock.Object, new PasswordService());
        // Given
        var emptyPassword = "";
        var newUser = new SignInCredentialsDto() { Email = "john@doe.com", Password = emptyPassword };
        // When
        var result = useCase.Invoking(useCase => useCase.Execute(newUser));
        // Then
        await result.Should().ThrowAsync<InvalidUserException>()
            .WithMessage("User password is required and cannot be empty");
    }

    [Fact]
    async Task ShortPassword_ThrowsInvalidUserException()
    {
        var useCase = new SignInUserUseCase(new UserValidator(), this.userDAMock.Object, new PasswordService());
        // Given
        var shortPassword = "00";
        var newUser = new SignInCredentialsDto() { Email = "john@doe.com", Password = shortPassword };
        // When
        var result = useCase.Invoking(useCase => useCase.Execute(newUser));
        // Then
        await result.Should().ThrowAsync<InvalidUserException>()
            .WithMessage("User password must be between 3 and 32 characters");
    }

    [Fact]
    async Task LongPassword_ThrowsInvalidUserException()
    {
        var useCase = new SignInUserUseCase(new UserValidator(), this.userDAMock.Object, new PasswordService());
        // Given
        var longPassword = "000000000000000000000000000000000";
        var newUser = new SignInCredentialsDto() { Email = "john@doe.com", Password = longPassword };
        // When
        var result = useCase.Invoking(useCase => useCase.Execute(newUser));
        // Then
        await result.Should().ThrowAsync<InvalidUserException>()
            .WithMessage("User password must be between 3 and 32 characters");
    }

    [Fact]
    async Task UserNotFoundByEmail_ThrowsUserNotFoundException()
    {
        var email = "john@doe.com";
        var user = await this.userDAMock.Object.FindByEmail(email);
        var useCase = new SignInUserUseCase(new UserValidator(), this.userDAMock.Object, new PasswordService());
        // Given
        var credentials = new SignInCredentialsDto() { Email = email, Password = "1234" };
        user.Should().BeNull();
        // When
        var result = useCase.Invoking(useCase => useCase.Execute(credentials));
        // Then
        await result.Should().ThrowAsync<UserNotFoundException>()
            .WithMessage("User not found with the e-mail passed");
    }

    [Fact]
    async Task WrongPassword_ThrowsPasswordNotMatchException()
    {
        var email = "john@doe.com";
        var wrongPassword = "WRONG_PASSWORD";
        var userDb = new UserDbDto() { Id = Guid.NewGuid(),
                                       Name = "John Doe",
                                       Email = email,
                                       PasswordHash = "HASH" };
        this.userDAMock.Setup(mock => mock.FindByEmail(email)).ReturnsAsync(userDb);
        var user = await this.userDAMock.Object.FindByEmail(email);
        this.passServMock.Setup(mock => mock.MatchPasswordAndHash(wrongPassword, userDb.PasswordHash))
            .ReturnsAsync(false);
        var useCase = new SignInUserUseCase(new UserValidator(), this.userDAMock.Object, this.passServMock.Object);
        // Given
        var credentials = new SignInCredentialsDto() { Email = email, Password = wrongPassword };
        user.Should().NotBeNull();
        user?.Email.Should().BeSameAs(email);
        // When
        var result = useCase.Invoking(useCase => useCase.Execute(credentials));
        // Then
        await result.Should().ThrowAsync<PasswordNotMatchException>()
            .WithMessage("Password is not a match to the password hash");
    }
}

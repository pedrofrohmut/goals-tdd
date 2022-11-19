using System.Threading.Tasks;
using Goals.Core.Dtos;
using Xunit;
using FluentAssertions;
using Moq;
using System;

using Goals.Core.UseCases.Users;
using Goals.Core.Validators;
using Goals.Core.Exceptions;
using Goals.Core.DataAccess;
using Goals.Core.Services;

namespace Goals.Tests.Core.UseCases.Users;

public class SignUpUserUseCaseTest : IDisposable
{
    const string NAME     = "John Doe";
    const string EMAIL    = "john@doe.com";
    const string PASSWORD = "1234";
    const string HASH     = "HASH";

    // xUnit for some reason creates and destroys the class for each test
    // So you are suppose to use Constructor instead of Setup methods and
    // Dispose instead of clean up

    private readonly Mock<IUserDataAccess> userDAMock;
    private readonly Mock<IPasswordService> passServMock;
    private readonly ISignUpUserUseCase useCase;

    // Setup
    public SignUpUserUseCaseTest()
    {
        this.userDAMock = new Mock<IUserDataAccess>();
        this.passServMock = new Mock<IPasswordService>();
        this.useCase = new SignUpUserUseCase(new UserValidator(),
                                             this.userDAMock.Object,
                                             this.passServMock.Object);
    }

    // Clean Up
    public void Dispose() {}

    [Fact]
    async Task EmptyName_ThrowsInvalidUserException()
    {
        var emptyName = "";
        // Given
        var newUser = new CreateUserDto() { Name = emptyName, Email = EMAIL, Password = PASSWORD };
        // When
        var result = () => this.useCase.Execute(newUser);
        // Then
        await result.Should().ThrowAsync<InvalidUserException>()
            .WithMessage("User name is required and cannot be empty");
    }

    [Fact]
    async Task ShortLengthName_ThrowsInvalidUserException()
    {
        // Given
        var shortName = "jo";
        var newUser = new CreateUserDto() { Name = shortName, Email = EMAIL, Password = PASSWORD };
        // When
        var result = () => this.useCase.Execute(newUser);
        // Then
        await result.Should().ThrowAsync<InvalidUserException>()
            .WithMessage("User name must between 3 and 120 characters long");
    }

    [Fact]
    async Task LongLengthName_ThrowsInvalidUserException()
    {
        // Given
        var longName = "john000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
        var newUser = new CreateUserDto() { Name = longName, Email = EMAIL, Password = PASSWORD };
        // When
        var result = () => this.useCase.Execute(newUser);
        // Then
        await result.Should().ThrowAsync<InvalidUserException>()
            .WithMessage("User name must between 3 and 120 characters long");
    }

    [Fact]
    async Task EmptyEmail_ThrowsInvalidUserException()
    {
        // Given
        var emptyEmail = "";
        var newUser = new CreateUserDto() { Name = NAME, Email = emptyEmail, Password = PASSWORD };
        // When
        var result = () => this.useCase.Execute(newUser);
        // Then
        await result.Should().ThrowAsync<InvalidUserException>()
            .WithMessage("User e-mail is required and cannot be empty");
    }

    [Fact]
    async Task InvalidEmail_ThrowsInvalidUserException()
    {
        // Given
        var invalidEmail = "johndoe";
        var newUser = new CreateUserDto() { Name = NAME, Email = invalidEmail, Password = PASSWORD };
        // When
        var result = () => this.useCase.Execute(newUser);
        // Then
        await result.Should().ThrowAsync<InvalidUserException>()
            .WithMessage("User e-mail format is invalid");
    }

    [Fact]
    async Task EmptyPassword_ThrowsInvalidUserException()
    {
        // Given
        var emptyPassword = "";
        var newUser = new CreateUserDto() { Name = NAME, Email = EMAIL, Password = emptyPassword };
        // When
        var result = () => this.useCase.Execute(newUser);
        // Then
        await result.Should().ThrowAsync<InvalidUserException>()
            .WithMessage("User password is required and cannot be empty");
    }

    [Fact]
    async Task ShortPassword_ThrowsInvalidUserException()
    {
        // Given
        var shortPassword = "00";
        var newUser = new CreateUserDto() { Name = NAME, Email = EMAIL, Password = shortPassword };
        // When
        var result = () => this.useCase.Execute(newUser);
        // Then
        await result.Should().ThrowAsync<InvalidUserException>()
            .WithMessage("User password must be between 3 and 32 characters");
    }

    [Fact]
    async Task LongPassword_ThrowsInvalidUserException()
    {
        // Given
        var longPassword = "000000000000000000000000000000000";
        var newUser = new CreateUserDto() { Name = NAME, Email = EMAIL, Password = longPassword };
        // When
        var result = () => this.useCase.Execute(newUser);
        // Then
        await result.Should().ThrowAsync<InvalidUserException>()
            .WithMessage("User password must be between 3 and 32 characters");
    }

    [Fact]
    async Task EmailAlreadyInUse_ThrowsEmailAlreadyTakenException()
    {
        var userDb = new UserDbDto() { Id = Guid.NewGuid(), Name = NAME, Email = EMAIL, PasswordHash = HASH };
        this.userDAMock.Setup(dataAccess => dataAccess.FindByEmail(EMAIL)).ReturnsAsync(userDb);
        var user = await this.userDAMock.Object.FindByEmail(EMAIL);
        var newUser = new CreateUserDto() { Name = "Johnny 2", Email = EMAIL, Password = "pass123" };
        // Given
        user.Should().NotBeNull();
        // When
        var result = () => this.useCase.Execute(newUser);
        // Then
        await result.Should().ThrowAsync<EmailAlreadyTakenException>()
            .WithMessage("User e-mail is already registered and must be unique");
    }

    [Fact]
    async Task WithNoExceptions_UserIsCreated()
    {
        var user = await this.userDAMock.Object.FindByEmail(EMAIL);
        var newUser = new CreateUserDto() { Name = NAME, Email = EMAIL, Password = PASSWORD };
        // Given
        user.Should().BeNull();
        // When
        var result = () => this.useCase.Execute(newUser);
        // Then
        await result.Should().NotThrowAsync();
        passServMock.Verify(service => service.HashPassword(newUser.Password), Times.AtLeastOnce());
        userDAMock.Verify(dataAccess => dataAccess.Create(newUser, It.IsAny<string>()), Times.AtLeastOnce());
    }

}

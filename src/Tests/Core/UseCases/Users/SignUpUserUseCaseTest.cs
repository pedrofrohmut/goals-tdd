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
using Goals.Services;

namespace Goals.Tests.Core.UseCases.Users;

public class SignUpUserUseCaseTest : IDisposable
{
    // xUnit for some reason creates and destroys the class for each test
    // So you are suppose to use Constructor instead of Setup methods and
    // Dispose instead of clean up

    private readonly Mock<IUserDataAccess> userDAMock;
    private readonly Mock<IPasswordService> passServMock;

    // Setup
    public SignUpUserUseCaseTest()
    {
        this.userDAMock = new Mock<IUserDataAccess>();
        this.passServMock = new Mock<IPasswordService>();
    }

    // Clean Up
    public void Dispose() {}

    [Fact]
    async Task EmptyName_ThrowsInvalidUserException()
    {
        var useCase = new SignUpUserUseCase(new UserValidator(), this.userDAMock.Object, new PasswordService());
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
        var useCase = new SignUpUserUseCase(new UserValidator(), this.userDAMock.Object, new PasswordService());
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
        var useCase = new SignUpUserUseCase(new UserValidator(), this.userDAMock.Object, new PasswordService());
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
        var useCase = new SignUpUserUseCase(new UserValidator(), this.userDAMock.Object, new PasswordService());
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
        var useCase = new SignUpUserUseCase(new UserValidator(), this.userDAMock.Object, new PasswordService());
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
        var useCase = new SignUpUserUseCase(new UserValidator(), this.userDAMock.Object, new PasswordService());
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
        var useCase = new SignUpUserUseCase(new UserValidator(), this.userDAMock.Object, new PasswordService());
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
        var useCase = new SignUpUserUseCase(new UserValidator(), this.userDAMock.Object, new PasswordService());
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

    [Fact]
    async Task EmailAlreadyInUse_ThrowsEmailAlreadyTakenException()
    {
        var email = "john@doe.com";
        var foundUser = new UserDbDto() { Id = Guid.NewGuid(),
                                          Name = "John Doe",
                                          Email = email,
                                          PasswordHash = "HASH" };
        this.userDAMock.Setup(dataAccess => dataAccess.FindByEmail(email)).ReturnsAsync(foundUser);
        var user = await this.userDAMock.Object.FindByEmail(email);
        var newUser = new CreateUserDto() { Name = "Johnny", Email = email, Password = "1234" };
        var useCase = new SignUpUserUseCase(new UserValidator(), this.userDAMock.Object, new PasswordService());
        // Given
        user.Should().NotBeNull();
        // When
        var result = useCase.Invoking(useCase => useCase.Execute(newUser));
        // Then
        await result.Should().ThrowAsync<EmailAlreadyTakenException>()
            .WithMessage("User e-mail is already registered and must be unique");
    }

    [Fact]
    async Task WithNoExceptions_UserIsCreated()
    {
        var email = "john@doe.com";
        var user = await this.userDAMock.Object.FindByEmail(email);
        var newUser = new CreateUserDto() { Name = "Johnny", Email = email, Password = "1234" };
        var useCase = new SignUpUserUseCase(new UserValidator(), this.userDAMock.Object, this.passServMock.Object);
        // Given
        user.Should().BeNull();
        // When
        var result = useCase.Invoking(useCase => useCase.Execute(newUser));
        // Then
        await result.Should().NotThrowAsync();
        passServMock.Verify(service => service.HashPassword(newUser.Password), Times.AtLeastOnce());
        userDAMock.Verify(dataAccess => dataAccess.Create(newUser, It.IsAny<string>()), Times.AtLeastOnce());
    }

}

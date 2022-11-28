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

    [Theory]
    [InlineData("")]
    [InlineData("jo")]
    [InlineData("john000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000")]
    async Task ValidateName(string name)
    {
        var newUser = new CreateUserDto() { Name = name, Email = EMAIL, Password = PASSWORD };
        await this.useCase.Invoking(x => x.Execute(newUser)).Should().ThrowAsync<InvalidUserException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("johndoe.com")]
    async Task ValidateEmail(string email)
    {
        var newUser = new CreateUserDto() { Name = NAME, Email = email, Password = PASSWORD };
        await this.useCase.Invoking(x => x.Execute(newUser)).Should().ThrowAsync<InvalidUserException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("00")]
    [InlineData("000000000000000000000000000000000")]
    async Task ValidatePassword(string password)
    {
        var newUser = new CreateUserDto() { Name = NAME, Email = EMAIL, Password = password };
        await this.useCase.Invoking(x => x.Execute(newUser)).Should().ThrowAsync<InvalidUserException>();
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

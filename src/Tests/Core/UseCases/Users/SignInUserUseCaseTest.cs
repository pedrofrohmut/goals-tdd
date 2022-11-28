using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

using Goals.Core.DataAccess;
using Goals.Core.Dtos;
using Goals.Core.Exceptions;
using Goals.Core.Services;
using Goals.Core.UseCases.Users;
using Goals.Core.Validators;

namespace Goals.Tests.Core.UseCases.Users;

public class SignInUserUseCaseTest : IDisposable
{
    const string NAME     = "John Doe";
    const string EMAIL    = "john@doe.com";
    const string PASSWORD = "1234";
    const string HASH     = "HASH";
    const string TOKEN    = "TOKEN";

    private readonly Mock<IUserDataAccess> userDAMock;
    private readonly Mock<IPasswordService> passServMock;
    private readonly Mock<ITokenService> tokenServMock;
    private readonly ISignInUserUseCase useCase;
    private readonly UserDbDto userDb;

    // Setup
    // Mock services and data-access but keep every thing about the core without mocking
    public SignInUserUseCaseTest()
    {
        this.userDAMock = new Mock<IUserDataAccess>();
        this.passServMock = new Mock<IPasswordService>();
        this.tokenServMock = new Mock<ITokenService>();
        this.useCase = new SignInUserUseCase(new UserValidator(),
                                             this.userDAMock.Object,
                                             this.passServMock.Object,
                                             this.tokenServMock.Object);
        this.userDb = new UserDbDto() { Id = Guid.NewGuid(), Name = NAME, Email = EMAIL, PasswordHash = HASH };
    }

    // Clean Up
    public void Dispose() {}

    [Theory]
    [InlineData("")]
    [InlineData("johndoe")]
    async Task ValidateEmail(string email)
    {
        var credentials = new SignInCredentialsDto() { Email = email, Password = PASSWORD };
        await this.useCase.Invoking(x => x.Execute(credentials)).Should().ThrowAsync<InvalidUserException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("00")]
    [InlineData("000000000000000000000000000000000")]
    async Task ValidatePassword(string password)
    {
        var credentials = new SignInCredentialsDto() { Email = EMAIL, Password = password };
        await this.useCase.Invoking(x => x.Execute(credentials)).Should().ThrowAsync<InvalidUserException>();
    }

    [Fact]
    async Task UserNotFoundByEmail_ThrowsUserNotFoundException()
    {
        var user = await this.userDAMock.Object.FindByEmail(EMAIL);
        // Given
        var credentials = new SignInCredentialsDto() { Email = EMAIL, Password = PASSWORD };
        user.Should().BeNull();
        // When
        var result = () => this.useCase.Execute(credentials);
        // Then
        await result.Should().ThrowAsync<UserNotFoundException>()
            .WithMessage("User not found with the e-mail passed");
    }

    [Fact]
    async Task WrongPassword_ThrowsPasswordNotMatchException()
    {
        var wrongPassword = "WRONG_PASSWORD";
        this.userDAMock.Setup(mock => mock.FindByEmail(EMAIL)).ReturnsAsync(this.userDb);
        var user = await this.userDAMock.Object.FindByEmail(EMAIL);
        this.passServMock.Setup(mock => mock.MatchPasswordAndHash(wrongPassword, this.userDb.PasswordHash))
            .ReturnsAsync(false);
        // Given
        var credentials = new SignInCredentialsDto() { Email = EMAIL, Password = wrongPassword };
        user.Should().NotBeNull();
        user?.Email.Should().BeSameAs(EMAIL);
        // When
        var result = () => this.useCase.Execute(credentials);
        // Then
        await result.Should().ThrowAsync<PasswordNotMatchException>()
            .WithMessage("Password is not a match to the password hash");
    }

    [Fact]
    async Task UserExistsAndPasswordMatch_ReturnsSignedUser()
    {
        this.userDAMock.Setup(x => x.FindByEmail(EMAIL)).ReturnsAsync(this.userDb);
        var user = await this.userDAMock.Object.FindByEmail(EMAIL);
        this.passServMock.Setup(x => x.MatchPasswordAndHash(PASSWORD, this.userDb.PasswordHash)).ReturnsAsync(true);
        var isMatch = await this.passServMock.Object.MatchPasswordAndHash(PASSWORD, this.userDb.PasswordHash);
        this.tokenServMock.Setup(x => x.CreateToken(userDb.Id.ToString())).ReturnsAsync(TOKEN);
        // Given
        var credentials = new SignInCredentialsDto() { Email = EMAIL, Password = PASSWORD };
        user.Should().NotBeNull();
        isMatch.Should().BeTrue();
        // When
        var response = await this.useCase.Execute(credentials);
        // Then
        this.passServMock.Verify(x => x.MatchPasswordAndHash(PASSWORD, this.userDb.PasswordHash),
                                 Times.AtLeastOnce());
        this.tokenServMock.Verify(x => x.CreateToken(this.userDb.Id.ToString()), Times.AtLeastOnce());
        response.Should().NotBeNull();
        response.Id.Should().Be(this.userDb.Id.ToString());
        response.Name.Should().Be(this.userDb.Name);
        response.Email.Should().Be(this.userDb.Email);
        response.Token.Should().Be(TOKEN);
    }
}

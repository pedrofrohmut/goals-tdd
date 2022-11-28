using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

using Goals.Core.DataAccess;
using Goals.Core.Exceptions;
using Goals.Core.UseCases.Users;
using Goals.Core.Validators;
using System;
using Goals.Core.Dtos;

namespace Goals.Tests.Core.UseCases.Users;

// 1. authUserId is Empty >> throws InvalidUserException
// 2. Invalid authUserId >> throws InvalidUserException
// 2. user not found by authUserId >> throws UserNotFoundException
// 3. Valid authUserId and user found >> End with no return
public class VerifyUserUseCaseTest : IDisposable
{
    private readonly Mock<IUserDataAccess> userDataAccessMock;
    private readonly IVerifyUserUseCase verifyUserUseCase;

    // Setup
    // Mock services and data-access but keep every thing about the core without mocking
    public VerifyUserUseCaseTest()
    {
        this.userDataAccessMock = new Mock<IUserDataAccess>();
        this.verifyUserUseCase = new VerifyUserUseCase(new UserValidator(), this.userDataAccessMock.Object);
    }

    // Clean Up
    public void Dispose() {}

    [Fact]
    async Task EmptyAuthUserId_ThrowsInvalidUserException()
    {
        // Given
        var emptyAuthUserId = "";
        // When
        var result = () => this.verifyUserUseCase.Execute(emptyAuthUserId);
        // Then
        await result.Should().ThrowAsync<InvalidUserException>()
            .WithMessage("User id is required and cannot be empty");
    }

    [Fact]
    async Task InvalidAuthUserId_ThrowsInvalidUserException()
    {
        // Given
        var invalidAuthUserId = "INVALID_GUID";
        // When
        var result = () => this.verifyUserUseCase.Execute(invalidAuthUserId);
        // Then
        await result.Should().ThrowAsync<InvalidUserException>()
            .WithMessage("User id is not a valid GUID (globally unique identifier)");
    }

    [Fact]
    async Task UserNotFound_ThrowsUserNotFound()
    {
        var authUserId = Guid.NewGuid();
        var user = await this.userDataAccessMock.Object.FindById(authUserId);
        // Given
        user.Should().BeNull();
        // When
        var result = () => this.verifyUserUseCase.Execute(authUserId.ToString());
        // Then
        await result.Should().ThrowAsync<UserNotFoundException>()
            .WithMessage("User Not Found by authUserId");
    }

    [Fact]
    async Task ValidIdAndUserFound_DontThrow()
    {
        var validAuthUserId = Guid.NewGuid();
        var userDb = new UserDbDto() { Id = validAuthUserId,
                                       Name = "John Doe",
                                       Email = "john@doe.com",
                                       PasswordHash = "HASH" };
        this.userDataAccessMock.Setup(x => x.FindById(validAuthUserId)).ReturnsAsync(userDb);
        var user = await this.userDataAccessMock.Object.FindById(validAuthUserId);
        // Given
        user.Should().NotBeNull();
        // When
        var result = () => this.verifyUserUseCase.Execute(validAuthUserId.ToString());
        // Then
        await result.Should().NotThrowAsync();
        this.userDataAccessMock.Verify(x => x.FindById(validAuthUserId), Times.AtLeastOnce());
    }
}

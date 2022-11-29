using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

using Goals.Core.DataAccess;
using Goals.Core.Dtos;
using Goals.Core.Exceptions;
using Goals.Core.UseCases.Goals;
using Goals.Core.Validators;
using Moq;

namespace Goals.Tests.Core.UseCases.Goals;

// 1. Validate newGoal
// 2. Auth user not found => UserNotFoundException
// 3. Valid goal and user found => goal added
public class AddGoalUseCaseTest : IDisposable
{
    const string NAME  = "John Doe";
    const string EMAIL = "john@doe.com";
    const string HASH  = "HASH";
    const string TEXT  = "NEW GOAL TEXT";

    private readonly string authUserId;
    private readonly UserDbDto userDb;
    private readonly Mock<IUserDataAccess> userDAMock;
    private readonly Mock<IGoalDataAccess> goalDAMock;
    private readonly IAddGoalUseCase addGoalUseCase;

    // Setup
    public AddGoalUseCaseTest()
    {
        this.authUserId = Guid.NewGuid().ToString();
        this.userDb = new UserDbDto() { Id = Guid.Parse(this.authUserId),
                                        Name = NAME,
                                        Email = EMAIL,
                                        PasswordHash = HASH };
        this.userDAMock = new Mock<IUserDataAccess>();
        this.goalDAMock = new Mock<IGoalDataAccess>();
        this.addGoalUseCase = new AddGoalUseCase(new GoalValidator(),
                                                 this.userDAMock.Object,
                                                 this.goalDAMock.Object);
    }

    // Clean Up
    public void Dispose() {}

    [Theory]
    [InlineData("")]
    async Task ValidateNewGoal(string text)
    {
        var newGoal = new CreateGoalDto() { Text = text };
        await this.addGoalUseCase.Invoking(x => x.Execute(newGoal, this.authUserId))
            .Should().ThrowAsync<InvalidGoalException>();
    }

    [Fact]
    async Task AuthUserNotFound_ThrowsUserNotFound()
    {
        var user = await this.userDAMock.Object.FindById(this.authUserId);
        // Given
        user.Should().BeNull();
        var newGoal = new CreateGoalDto() { Text = TEXT };
        // When
        var result = () => this.addGoalUseCase.Execute(newGoal, this.authUserId);
        // Then
        await result.Should().ThrowAsync<UserNotFoundException>();
    }

    [Fact]
    async Task ValidGoalAndUserFound_ThrowsNoExceptions()
    {
        this.userDAMock.Setup(x => x.FindById(this.authUserId)).ReturnsAsync(this.userDb);
        var user = await this.userDAMock.Object.FindById(this.authUserId);
        // Given
        user.Should().NotBeNull();
        var newGoal = new CreateGoalDto() { Text = TEXT };
        // When
        var result = () => this.addGoalUseCase.Execute(newGoal, this.authUserId);
        // Then
        await result.Should().NotThrowAsync();
        this.userDAMock.Verify(x => x.FindById(this.authUserId), Times.Exactly(2));
        this.goalDAMock.Verify(x => x.Create(newGoal, this.authUserId), Times.Once());
    }
}

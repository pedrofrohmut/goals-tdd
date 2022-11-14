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
        await useCase.Invoking(useCase => useCase.Execute(newUser))
        // Then
            .Should().ThrowAsync<InvalidUserException>()
            .WithMessage("User is required and cannot be empty");
    }
}

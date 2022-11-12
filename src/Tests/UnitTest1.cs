using Xunit;
using Goals.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;

namespace Tests;

public class UnitTest1
{
    [Fact]
    void Hello()
    {
        var controller = new UsersController();
        var result = (OkObjectResult) controller.Hello("Bob");
        result.StatusCode.Should().Be(200);
        result.Value.Should().Be("Hello, Bob");
    }
}

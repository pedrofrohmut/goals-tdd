using Microsoft.AspNetCore.Mvc;

namespace Goals.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    [HttpGet("hello/{name}")]
    public ActionResult Hello(string name)
    {
        var message = $"Hello, {name}";
        return Ok(message);
    }
}

using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

[ApiController]
public class AuthController : ControllerBase
{
    public AuthController()
    {
    }

    [Microsoft.AspNetCore.Mvc.HttpPost("connect/token")]
    [Consumes("application/x-www-form-urlencoded")]
    [Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        return Ok(new { message = "Hello, World!" });      
    }
}

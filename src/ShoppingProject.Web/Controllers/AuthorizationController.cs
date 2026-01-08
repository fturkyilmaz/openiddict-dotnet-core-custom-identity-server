using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingProject.UseCases.Users;

namespace ShoppingProject.Web.Controllers;

[ApiController]
[Route("auth")]
public class AuthorizationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthorizationController(IMediator mediator) => _mediator = mediator;

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var id = await _mediator.Send(command);
        return Created("/auth/register", new { user_id = id, command.UserName, command.Email });
    }

    [HttpPost("token")]
    [AllowAnonymous]
    public async Task<IActionResult> Token([FromBody] LoginUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("revoke")]
    [Authorize]
    public async Task<IActionResult> Revoke([FromBody] RevokeTokenCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var result = await _mediator.Send(new MeQuery(User));
        return Ok(result);
    }
}

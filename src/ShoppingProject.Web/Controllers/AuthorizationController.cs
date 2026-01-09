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
    [Consumes("application/x-www-form-urlencoded")] 
    public async Task<IActionResult> Token([FromForm] string username, [FromForm] string password) 
    { 
        var command = new LoginUserCommand(username, password); 
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

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("logout-everywhere")] 
    [Authorize] 
    public async Task<IActionResult> LogoutEverywhere(string userId) 
    { 
        await _mediator.Send(new LogoutEverywhereCommand(userId)); 
        return Ok(new { message = "Tüm cihazlardan başarıyla çıkış yapıldı." }); 
    }

}

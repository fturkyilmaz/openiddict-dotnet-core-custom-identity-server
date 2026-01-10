using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingProject.UseCases.Users;
using ShoppingProject.UseCases.Users.Login;
using System.Security.Claims; 
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;


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

    [HttpPost("revoke")]
    [Authorize]
    public async Task<IActionResult> Revoke([FromBody] RevokeTokenCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    // [HttpPost("token")]
    // [AllowAnonymous]
    // public async Task<IActionResult> Token()
    // {
    //     var response = HttpContext.GetOpenIddictServerResponse();
    //     if (response == null) { return BadRequest(new { error = "No OpenIddict response available" }); } 
    //     return new ObjectResult(response) { StatusCode = response.StatusCode };
    // }

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

    // Şifre değiştirme 
    [HttpPost("change-password")] 
    [Authorize] 
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command) { 
        await _mediator.Send(command); 
        return Ok(new { message = "Şifre başarıyla değiştirildi." }); 
    } 
    
    // Şifre sıfırlama link/token üretme 
    [HttpPost("forgot-password")] 
    [AllowAnonymous] 
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command) { 
        await _mediator.Send(command); 
        return Ok(new { message = "Şifre sıfırlama linki/tokeni gönderildi." }); 
    } 
    
    // Reset token ile yeni şifre belirleme 
    [HttpPost("reset-password")] 
    [AllowAnonymous] 
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command) { 
        await _mediator.Send(command); 
        return Ok(new { message = "Şifre başarıyla sıfırlandı." }); 
    } 
    
    // E‑mail doğrulama 
    [HttpPost("verify-email")] 
    [AllowAnonymous] 
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand command) { 
        await _mediator.Send(command); 
        return Ok(new { message = "E‑mail doğrulandı." }); 
    } 
    
    // Doğrulama mailini tekrar gönderme 
    [HttpPost("resend-verification")] 
    [AllowAnonymous] 
    public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationCommand command) { 
        await _mediator.Send(command); 
        return Ok(new { message = "Doğrulama maili tekrar gönderildi." }); 
    } 
    
    // İki faktörlü kimlik doğrulamayı aktif etme 
    [HttpPost("enable-2fa")] 
    [Authorize] 
    public async Task<IActionResult> Enable2FA([FromBody] Enable2FACommand command) { 
        await _mediator.Send(command); 
        return Ok(new { message = "İki faktörlü kimlik doğrulama aktif edildi." }); 
    } 
    
    // İki faktörlü kimlik doğrulamayı devre dışı bırakma 
    [HttpPost("disable-2fa")] 
    [Authorize] 
    public async Task<IActionResult> Disable2FA([FromBody] Disable2FACommand command) { 
        await _mediator.Send(command); 
        return Ok(new { message = "İki faktörlü kimlik doğrulama devre dışı bırakıldı." }); 
    }

}

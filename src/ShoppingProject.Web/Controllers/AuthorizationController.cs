using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingProject.UseCases.Users.Queries.Me;
using ShoppingProject.UseCases.Users.Commands.Login;
using ShoppingProject.UseCases.Users.Commands.Register;
using ShoppingProject.UseCases.Users.Commands.RefreshToken;
using ShoppingProject.UseCases.Users.Commands.TwoFactor;
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
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
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
    
    // [HttpGet("me")]
    // [Authorize]
    // public async Task<IActionResult> Me()
    // {
    //     var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //     var user = await _mediator.Send(new MeQuery(userId));
    //     return Ok(user);
    // }

    [HttpGet("me")] 
    [Authorize]
    public async Task<IActionResult> Me() { 
        var user = HttpContext.User; 

        if (user?.Identity?.IsAuthenticated != true) 
        { 
            return Unauthorized(); 
        } 

        var userId =user.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        if (userId == null) { return BadRequest(new { error = "No user ID found" }); } 
        var result = await _mediator.Send(new MeQuery(userId));

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
    public async Task<IActionResult> LogoutEverywhere([FromForm] string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return BadRequest(new { error = "userId required" });

        var tokenManager = HttpContext.RequestServices.GetRequiredService<IOpenIddictTokenManager>();

        // Kullanıcının tüm tokenlarını bul ve revoke et
        await foreach (var token in tokenManager.FindBySubjectAsync(userId))
        {
            await tokenManager.TryRevokeAsync(token);
        }

        return Ok(new { message = "All tokens revoked for user " + userId });
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

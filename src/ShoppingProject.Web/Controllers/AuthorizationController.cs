using Microsoft.AspNetCore.Mvc;

namespace ShoppingProject.Web.Controllers;

[ApiController]
[Route("auth")]
public class AuthorizationController : ControllerBase
{
    [HttpPost("register")]
    public IActionResult Register([FromBody] object request)
    {
        // Mock response - gerçek uygulamada kullanıcı DB'ye kaydedilir
        var response = new
        {
            user_id = Guid.NewGuid(),
            username = "mockuser",
            email = "mockuser@example.com",
            message = "User registered successfully"
        };
        return Created("/auth/register", response);
    }

    [HttpPost("token")]
    public IActionResult Token([FromBody] object request)
    {
        // Mock response - gerçek uygulamada kullanıcı doğrulama ve token oluşturma yapılır
        var response = new
        {
            access_token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
            token_type = "Bearer",
            expires_in = 3600,
            refresh_token = "def50200a1b2c3d4e5f6..."
        };
        return Ok(response);
    }

    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] object request)
    {
        // Mock response - gerçek uygulamada refresh token doğrulanır ve yeni access token üretilir
        var response = new
        {
            access_token = "newAccessToken123...",
            token_type = "Bearer",
            expires_in = 3600,
            refresh_token = "newRefreshToken456..."
        };
        return Ok(response);
    }

    [HttpPost("revoke")]
    public IActionResult Revoke([FromBody] object request)
    {
        // Mock response - gerçek uygulamada refresh token veya authorization revoke edilir
        return NoContent();
    }

    [HttpGet("me")]
    public IActionResult Me()
    {
        // Mock response - gerçek uygulamada ClaimsPrincipal'dan user bilgileri alınır
        var response = new
        {
            user_id = Guid.NewGuid(),
            username = "mockuser",
            email = "mockuser@example.com",
            roles = new[] { "User", "Admin" }
        };
        return Ok(response);
    }
}

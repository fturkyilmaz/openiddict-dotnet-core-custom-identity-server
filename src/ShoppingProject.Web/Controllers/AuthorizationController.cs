using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using ShoppingProject.Infrastructure.Auth;

namespace ShoppingProject.Web.Controllers;

[ApiController]
[Route("auth")]
public class AuthorizationController : ControllerBase
{
    [HttpPost("token")]
    public IActionResult Token()
    {
        // Mock Data - Gerçek uygulamada kullanıcı doğrulama ve token oluşturma işlemleri burada yapılır.

        var response = new
        {
            access_token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...", // JWT string
            token_type = "Bearer",
            expires_in = 3600, // saniye cinsinden (ör. 1 saat)
            refresh_token = "def50200a1b2c3d4e5f6..." // opsiyonel
        };

        return Ok(response);
    }
}

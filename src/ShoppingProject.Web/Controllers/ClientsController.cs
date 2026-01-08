using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingProject.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/clients")]
[ApiVersion("1.0")]
public class ClientsController : ControllerBase
{
    public ClientsController()
    {
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        // Mock data list
        var clients = new List<object>
        {
            new { Id = Guid.NewGuid(), ClientId = "client-001", DisplayName = "Alpha Corp" },
            new { Id = Guid.NewGuid(), ClientId = "client-002", DisplayName = "Beta Solutions" },
            new { Id = Guid.NewGuid(), ClientId = "client-003", DisplayName = "Gamma Tech" }
        };

        // Simulate async
        await Task.Delay(50);

        return Ok(clients);
    }
}

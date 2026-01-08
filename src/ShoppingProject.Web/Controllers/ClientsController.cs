using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingProject.Infrastructure.Data;

namespace ShoppingProject.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/clients")]
[ApiVersion("1.0")]
public class ClientsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ClientsController(AppDbContext db) => _db = db;

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var clients = await _db.Clients
            .Select(c => new { c.Id, c.ClientId, c.DisplayName })
            .ToListAsync();

        return Ok(clients);
    }
}

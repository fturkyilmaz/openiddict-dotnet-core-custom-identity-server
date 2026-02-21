using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingProject.Infrastructure.Data;
using ShoppingProject.Infrastructure.Auth;
using ShoppingProject.Core.UserAggregate;
using ShoppingProject.UseCases.Users.Queries.Search;

namespace ShoppingProject.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/users")]
[ApiVersion("1.0")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;

    private readonly IMediator _mediator;

    public UsersController(IMediator mediator, AppDbContext db)
    {
        _mediator = mediator;
        _db = db;
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SearchUsers(
        [FromQuery] string? search,
        [FromQuery] int? status,
        [FromQuery] bool includeClaims = false)
    {
        var result = await _mediator.Send(
            new SearchQuery(search, status, includeClaims)
        );

        return Ok(result);
    }

}





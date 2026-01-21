using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingProject.UseCases.Users.Queries.Me;
using ShoppingProject.UseCases.Users.Commands.Login;
using ShoppingProject.UseCases.Users.Commands.Register;
using ShoppingProject.UseCases.Users.Commands.RefreshToken;
using ShoppingProject.UseCases.Users.Commands.TwoFactor;
using System.Security.Claims; 
using ShoppingProject.Infrastructure.Data;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Microsoft.EntityFrameworkCore;
using ShoppingProject.Core.UserAggregate;

[ApiController]
[Route("api/v{version:apiVersion}/{entity}")]
[ApiVersion("1.0")]
public class GenericCrudController<TEntity> : ControllerBase where TEntity : class
{
    private readonly AppDbContext _db;
    private readonly DbSet<TEntity> _set;

    public GenericCrudController(AppDbContext db)
    {
        _db = db;
        _set = db.Set<TEntity>();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _set.ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var entity = await _set.FindAsync(id);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TEntity entity)
    {
        _set.Add(entity);
        await _db.SaveChangesAsync();
        return Ok(entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] TEntity entity)
    {
        _db.Entry(entity).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return Ok(entity);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var entity = await _set.FindAsync(id);
        if (entity is null) return NotFound();
        _set.Remove(entity);
        await _db.SaveChangesAsync();
        return Ok();
    }
}

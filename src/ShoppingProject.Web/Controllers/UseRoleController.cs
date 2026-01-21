using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingProject.Infrastructure.Data;
using ShoppingProject.Core.UserAggregate;
using ShoppingProject.UseCases.UserRoles.DTOs;

namespace ShoppingProject.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/user-roles")]
[ApiVersion("1.0")]
public class UseRoleController : ControllerBase
{
    private readonly AppDbContext _db;

    public UseRoleController(AppDbContext db) => _db = db;

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var userRoles = await _db.UserRoles
            .Where(ur => !ur.IsDeleted)
            .Select(ur => new
            {
                ur.Id,
                ur.UserId,
                ur.RoleId,
                User = _db.Users.Where(u => u.Id == ur.UserId).Select(u => u.DisplayName).FirstOrDefault(),
                Role = _db.Roles.Where(r => r.Id == ur.RoleId).Select(r => r.Name).FirstOrDefault()
            })
            .ToListAsync();

        return Ok(userRoles);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] UserRoleDto userRoleDto)
    {
        var alreadyAssigned = await _db.UserRoles
            .AnyAsync(ur => ur.UserId == userRoleDto.UserId && ur.RoleId == userRoleDto.RoleId && !ur.IsDeleted);
        if (alreadyAssigned)
            return BadRequest("User already has this role");

        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = userRoleDto.UserId,
            RoleId = userRoleDto.RoleId,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            IsDeleted = false
        };

        _db.UserRoles.Add(userRole);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Role assigned successfully", userRole });
    }

    [HttpDelete]    
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete([FromBody] UserRoleDto userRoleDto)
    {
        var userRole = await _db.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userRoleDto.UserId && ur.RoleId == userRoleDto.RoleId && !ur.IsDeleted);

        if (userRole is null)
            return NotFound("User does not have this role");

        userRole.IsDeleted = true;
        userRole.UpdatedDate = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new { message = "Role removed successfully", userRole });
    }
}

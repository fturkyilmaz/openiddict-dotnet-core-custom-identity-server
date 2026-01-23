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
public class UserRoleController : ControllerBase
{
    private readonly AppDbContext _db;

    public UserRoleController(AppDbContext db) => _db = db;

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

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] string roleName)
    {
        var role = await _db.UserRoles.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        if (role is null) return NotFound();

        role.RoleId = id;
        role.UpdatedDate = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(role);
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

    [HttpGet("by-user/{userId}")]
    public async Task<IActionResult> GetByUser(Guid userId)
    {
        var roles = await _db.UserRoles
            .Where(ur => ur.UserId == userId && !ur.IsDeleted)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        return Ok(roles);
    }

    [HttpGet("by-role/{roleId}")]
    public async Task<IActionResult> GetByRole(Guid roleId)
    {
        var users = await _db.UserRoles
            .Where(ur => ur.RoleId == roleId && !ur.IsDeleted)
            .Select(ur => ur.UserId)
            .ToListAsync();

        return Ok(users);
    }

}

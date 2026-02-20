using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingProject.Infrastructure.Data;
using ShoppingProject.Core.UserAggregate;
using OpenIddict.Abstractions;
using System.Security.Claims;

namespace ShoppingProject.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/admin")]
[ApiVersion("1.0")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictScopeManager _scopeManager;

    public AdminController(
        AppDbContext db, 
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictScopeManager scopeManager)
    {
        _db = db;
        _applicationManager = applicationManager;
        _scopeManager = scopeManager;
    }

    #region Users

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = _db.Users.AsQueryable();
        
        var total = await query.CountAsync();
        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new 
            {
                u.Id,
                u.UserName,
                u.Email,
                u.DisplayName,
                u.IsActive,
                u.CreatedAt,
                u.UpdatedAt
            })
            .ToListAsync();

        return Ok(new { data = users, total, page, pageSize });
    }

    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        var roles = await _db.UserRoles
            .Where(ur => ur.UserId == id && !ur.IsDeleted)
            .Join(_db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
            .ToListAsync();

        return Ok(new 
        { 
            user.Id, 
            user.UserName, 
            user.Email, 
            user.DisplayName,
            user.IsActive,
            user.CreatedAt,
            user.UpdatedAt,
            roles
        });
    }

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName || u.Email == request.Email);
        if (existingUser != null)
            return BadRequest("User already exists");

        var passwordHasher = new Infrastructure.Auth.PasswordHasher();
        var hash = passwordHasher.HashPassword(request.Password);

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = request.UserName,
            Email = request.Email,
            DisplayName = request.DisplayName,
            PasswordHash = hash,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Created($"/api/v1/admin/users/{user.Id}", new { user.Id, user.UserName, user.Email });
    }

    [HttpPut("users/{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        if (!string.IsNullOrEmpty(request.DisplayName))
            user.DisplayName = request.DisplayName;
        
        if (!string.IsNullOrEmpty(request.Email))
            user.Email = request.Email;

        if (request.IsActive.HasValue)
            user.IsActive = request.IsActive.Value;

        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(new { user.Id, user.UserName, user.Email, user.DisplayName, user.IsActive });
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        // Soft delete
        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        
        await _db.SaveChangesAsync();

        return Ok(new { message = "User deactivated successfully" });
    }

    [HttpPost("users/{id}/change-password")]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordRequest request)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        var passwordHasher = new Infrastructure.Auth.PasswordHasher();
        user.PasswordHash = passwordHasher.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new { message = "Password changed successfully" });
    }

    #endregion

    #region Roles

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _db.Roles
            .Select(r => new 
            {
                r.Id,
                r.Name,
                r.Description,
                UserCount = _db.UserRoles.Count(ur => ur.RoleId == r.Id && !ur.IsDeleted)
            })
            .ToListAsync();

        return Ok(roles);
    }

    [HttpGet("roles/{id}")]
    public async Task<IActionResult> GetRole(Guid id)
    {
        var role = await _db.Roles.FindAsync(id);
        if (role == null) return NotFound();

        var users = await _db.UserRoles
            .Where(ur => ur.RoleId == id && !ur.IsDeleted)
            .Join(_db.Users, ur => ur.UserId, u => u.Id, (ur, u) => new { u.Id, u.UserName, u.Email, u.DisplayName })
            .ToListAsync();

        return Ok(new { role.Id, role.Name, role.Description, users });
    }

    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
    {
        var existingRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == request.Name);
        if (existingRole != null)
            return BadRequest("Role already exists");

        var role = new ApplicationRole
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description
        };

        _db.Roles.Add(role);
        await _db.SaveChangesAsync();

        return Created($"/api/v1/admin/roles/{role.Id}", new { role.Id, role.Name });
    }

    [HttpPut("roles/{id}")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleRequest request)
    {
        var role = await _db.Roles.FindAsync(id);
        if (role == null) return NotFound();

        if (!string.IsNullOrEmpty(request.Name))
            role.Name = request.Name;
        
        if (request.Description != null)
            role.Description = request.Description;

        await _db.SaveChangesAsync();

        return Ok(new { role.Id, role.Name, role.Description });
    }

    [HttpDelete("roles/{id}")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        var role = await _db.Roles.FindAsync(id);
        if (role == null) return NotFound();

        // Remove all user-role associations
        var userRoles = await _db.UserRoles.Where(ur => ur.RoleId == id).ToListAsync();
        _db.UserRoles.RemoveRange(userRoles);

        _db.Roles.Remove(role);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Role deleted successfully" });
    }

    #endregion

    #region User-Role Assignments

    [HttpGet("user-roles")]
    public async Task<IActionResult> GetUserRoles()
    {
        var userRoles = await _db.UserRoles
            .Where(ur => !ur.IsDeleted)
            .Select(ur => new 
            {
                ur.UserId,
                ur.RoleId,
                UserName = _db.Users.Where(u => u.Id == ur.UserId).Select(u => u.UserName).FirstOrDefault(),
                RoleName = _db.Roles.Where(r => r.Id == ur.RoleId).Select(r => r.Name).FirstOrDefault()
            })
            .ToListAsync();

        return Ok(userRoles);
    }

    [HttpPost("user-roles")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
    {
        var alreadyAssigned = await _db.UserRoles
            .AnyAsync(ur => ur.UserId == request.UserId && ur.RoleId == request.RoleId && !ur.IsDeleted);
        
        if (alreadyAssigned)
            return BadRequest("User already has this role");

        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            RoleId = request.RoleId,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            IsDeleted = false
        };

        _db.UserRoles.Add(userRole);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Role assigned successfully" });
    }

    [HttpDelete("user-roles")]
    public async Task<IActionResult> RemoveRole([FromBody] AssignRoleRequest request)
    {
        var userRole = await _db.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == request.UserId && ur.RoleId == request.RoleId && !ur.IsDeleted);

        if (userRole == null)
            return NotFound("User does not have this role");

        userRole.IsDeleted = true;
        userRole.UpdatedDate = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new { message = "Role removed successfully" });
    }

    #endregion

    #region OpenIddict Applications

    [HttpGet("applications")]
    public async Task<IActionResult> GetApplications()
    {
        var applications = new List<object>();
        
        await foreach (var app in _applicationManager.ListAsync())
        {
            applications.Add(new 
            {
                ClientId = await _applicationManager.GetClientIdAsync(app),
                DisplayName = await _applicationManager.GetDisplayNameAsync(app),
                ClientType = await _applicationManager.GetClientTypeAsync(app)
            });
        }

        return Ok(applications);
    }

    [HttpGet("applications/{clientId}")]
    public async Task<IActionResult> GetApplication(string clientId)
    {
        var app = await _applicationManager.FindByClientIdAsync(clientId);
        if (app == null) return NotFound();
        
        return Ok(new 
        {
            ClientId = await _applicationManager.GetClientIdAsync(app),
            DisplayName = await _applicationManager.GetDisplayNameAsync(app),
            ClientType = await _applicationManager.GetClientTypeAsync(app)
        });
    }

    #endregion

    #region OpenIddict Scopes

    [HttpGet("scopes")]
    public async Task<IActionResult> GetScopes()
    {
        var scopes = new List<object>();
        
        await foreach (var scope in _scopeManager.ListAsync())
        {
            scopes.Add(new 
            {
                Name = await _scopeManager.GetNameAsync(scope),
                DisplayName = await _scopeManager.GetDisplayNameAsync(scope),
                Description = await _scopeManager.GetDescriptionAsync(scope)
            });
        }

        return Ok(scopes);
    }

    #endregion

    #region Dashboard Stats

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var totalUsers = await _db.Users.CountAsync();
        var activeUsers = await _db.Users.CountAsync(u => u.IsActive);
        var totalRoles = await _db.Roles.CountAsync();
        
        var applications = new List<object>();
        await foreach (var app in _applicationManager.ListAsync())
        {
            applications.Add(app);
        }

        return Ok(new 
        {
            totalUsers,
            activeUsers,
            totalRoles,
            totalApplications = applications.Count
        });
    }

    #endregion
}

#region Request DTOs

public record CreateUserRequest(string UserName, string Email, string Password, string DisplayName);
public record UpdateUserRequest(string? DisplayName, string? Email, bool? IsActive);
public record ChangePasswordRequest(string NewPassword);

public record CreateRoleRequest(string Name, string Description);
public record UpdateRoleRequest(string? Name, string? Description);

public record AssignRoleRequest(Guid UserId, Guid RoleId);

#endregion

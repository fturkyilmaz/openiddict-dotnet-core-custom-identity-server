using ShoppingProject.Core.Common;
using Ardalis.SharedKernel;

namespace ShoppingProject.Core.UserAggregate;

public class ApplicationUser : BaseEntity<Guid>, IAggregateRoot
{
    public string UserName { get; set; } = default!;
    public string DisplayName { get; set; } = String.Empty;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
    public ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();
}

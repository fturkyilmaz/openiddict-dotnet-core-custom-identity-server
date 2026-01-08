namespace ShoppingProject.Core.UserAggregate;

public class UserRole
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = default!;
    public Guid RoleId { get; set; }
    public ApplicationRole Role { get; set; } = default!;
}

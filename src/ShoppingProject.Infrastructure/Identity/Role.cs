namespace ShoppingProject.Infrastructure.Identity;
public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    
    public ICollection<UserRole> Users { get; set; } = new List<UserRole>();
}

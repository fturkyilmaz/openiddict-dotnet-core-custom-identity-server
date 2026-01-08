namespace ShoppingProject.Infrastructure.Identity
{
  public class ApplicationUser
  {
      public Guid Id { get; set; }
      public string Username { get; set; } = default!;
      public string Email { get; set; } = default!;
      public string PasswordHash { get; set; } = default!;
      public string PasswordSalt { get; set; } = default!;
      public bool IsActive { get; set; } = true;
      public DateTime CreatedAt { get; set; }
      public DateTime UpdatedAt { get; set; }

      public ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
      public ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();
    }
}

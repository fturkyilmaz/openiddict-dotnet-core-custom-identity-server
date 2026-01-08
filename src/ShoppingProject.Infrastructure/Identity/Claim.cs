namespace ShoppingProject.Infrastructure.Identity;

public class Claim
{
    public Guid Id { get; set; }
    public string Type { get; set; } = default!;
    public string Description { get; set; } = default!;
    
    public ICollection<UserClaim> UserClaims { get; set; } = new List<UserClaim>();
}

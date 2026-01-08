namespace ShoppingProject.Infrastructure.Identity;

public class Token
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
    
    public Guid ClientId { get; set; }
    public Client Client { get; set; } = default!;
    
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
}

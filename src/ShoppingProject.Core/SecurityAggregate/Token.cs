namespace ShoppingProject.Core.SecurityAggregate;

public class Token
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
}

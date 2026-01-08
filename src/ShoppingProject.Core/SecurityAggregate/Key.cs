namespace ShoppingProject.Core.SecurityAggregate;

public class Key
{
    public Guid Id { get; set; }
    public string Kid { get; set; } = default!;
    public string PublicKey { get; set; } = default!;
    public string PrivateKey { get; set; } = default!;
    public string Algorithm { get; set; } = "RS256";
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}

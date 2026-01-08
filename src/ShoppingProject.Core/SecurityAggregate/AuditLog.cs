namespace ShoppingProject.Core.SecurityAggregate;

public class AuditLog
{
    public Guid Id { get; set; }
    public string? UserId { get; set; }
    public string Action { get; set; } = default!;
    public string? ClientId { get; set; }
    public DateTime Timestamp { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}

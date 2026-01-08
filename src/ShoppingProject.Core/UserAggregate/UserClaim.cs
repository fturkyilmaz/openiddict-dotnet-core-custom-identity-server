namespace ShoppingProject.Core.UserAggregate;

public class UserClaim
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string Value { get; set; } = default!;
}

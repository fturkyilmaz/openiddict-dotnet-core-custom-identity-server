using ShoppingProject.Core.Common;

namespace ShoppingProject.Core.OpenIdAggregate;

public class Client:BaseEntity<Guid>
{
    public string ClientId { get; set; } = default!;
    public string ClientSecretHash { get; set; } = default!;
    public string RedirectUrisJson { get; set; } = default!;
    public string AllowedScopesJson { get; set; } = default!;
    public bool IsActive { get; set; } = true;
}

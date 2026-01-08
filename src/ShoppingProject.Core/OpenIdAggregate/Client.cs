namespace ShoppingProject.Core.OpenIdAggregate;

public class Client
{
    public Guid Id { get; set; }
    public string ClientId { get; set; } = default!;
    public string ClientSecretHash { get; set; } = default!;
    public string RedirectUrisJson { get; set; } = default!;
    public string AllowedScopesJson { get; set; } = default!;
    public bool IsActive { get; set; } = true;
}

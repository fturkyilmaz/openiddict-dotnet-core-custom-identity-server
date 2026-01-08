using ShoppingProject.Core.Common;

namespace ShoppingProject.Core.OpenIdAggregate;

public class ClientScope:BaseEntity<Guid>
{
    public Guid ClientId { get; set; }
    public Client Client { get; set; } = default!;
    public Guid ScopeId { get; set; }
    public Scope Scope { get; set; } = default!;
}

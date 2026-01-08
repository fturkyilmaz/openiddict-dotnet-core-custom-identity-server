using Microsoft.EntityFrameworkCore;
using System.Reflection;
using ShoppingProject.Core.ContributorAggregate;
using ShoppingProject.Core.UserAggregate;
using ShoppingProject.Core.SecurityAggregate;
using ShoppingProject.Core.OpenIdAggregate;
using OpenIddict.EntityFrameworkCore.Models;

namespace ShoppingProject.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Contributor> Contributors => Set<Contributor>();

    // UserAggregate
    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<ApplicationRole> Roles => Set<ApplicationRole>();
    public DbSet<UserClaim> UserClaims => Set<UserClaim>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    // SecurityAggregate
    public DbSet<Token> Tokens => Set<Token>();
    public DbSet<Key> Keys => Set<Key>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    // OpenIdAggregate
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Scope> Scopes => Set<Scope>();
    public DbSet<ClientScope> ClientScopes => Set<ClientScope>();

    // OpenIddict
    public DbSet<OpenIddictEntityFrameworkCoreApplication> OpenIddictApplications => Set<OpenIddictEntityFrameworkCoreApplication>();
    public DbSet<OpenIddictEntityFrameworkCoreAuthorization> OpenIddictAuthorizations => Set<OpenIddictEntityFrameworkCoreAuthorization>();
    public DbSet<OpenIddictEntityFrameworkCoreScope> OpenIddictScopes => Set<OpenIddictEntityFrameworkCoreScope>();
    public DbSet<OpenIddictEntityFrameworkCoreToken> OpenIddictTokens => Set<OpenIddictEntityFrameworkCoreToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override int SaveChanges() =>
        SaveChangesAsync().GetAwaiter().GetResult();
}

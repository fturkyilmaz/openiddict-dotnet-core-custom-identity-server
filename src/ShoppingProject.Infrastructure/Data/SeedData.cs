using Bogus;
using ShoppingProject.Core.ContributorAggregate;
using ShoppingProject.Core.UserAggregate;
using ShoppingProject.Core.OpenIdAggregate;
using ShoppingProject.Infrastructure.Auth;
using OpenIddict.Abstractions;

namespace ShoppingProject.Infrastructure.Data;

public static class SeedData
{
    public const int NUMBER_OF_USERS = 10;
    private static Guid adminUserId;

    public static async Task InitializeAsync(IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();


       // OpenIddict Manager
        var applicationManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

        if (!await dbContext.Users.AnyAsync())
        {
            var passwordHasher = new PasswordHasher();
            var hash = passwordHasher.HashPassword("Password123!");

            dbContext.Users.Add(new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "admin",
                Email = "admin@test.com",
                DisplayName = "Administrator",
                PasswordHash = hash
            });
        }else { adminUserId = await dbContext.Users.Where(u => u.UserName == "admin").Select(u => u.Id).FirstAsync(); }

        if (!dbContext.Clients.Any())
        {
            dbContext.Clients.Add(new Client
            {
                Id = Guid.NewGuid(),
                ClientId = "shopping-admin",
                ClientSecretHash = "dev-secret",
                RedirectUrisJson = "[\"https://localhost:5001/signin-oidc\"]",
                AllowedScopesJson = "[\"api\", \"openid\", \"profile\"]",
            });
        }

        if (!dbContext.Scopes.Any())
        {
            dbContext.Scopes.Add(new Scope
            {
                Id = Guid.NewGuid(),
                Name = "api",
                Description = "Shopping API"
            });
        }

        if (!dbContext.Roles.Any())
        {
            dbContext.Roles.AddRange(
                new ApplicationRole { Name = "Admin", Description = "Administrator role with full permissions." },
                new ApplicationRole { Name = "User", Description = "Standard user role with limited permissions." }
            );
        }

       if (await scopeManager.FindByNameAsync("api") is null)
        {
            await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "api",
                DisplayName = "API Access",
                Resources = { "resource_server_1" }
            });
        }


        if (await applicationManager.FindByClientIdAsync("shopping-admin") == null)
        {
            await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "shopping-admin",
                ClientSecret = "dev-secret",
                DisplayName = "Shopping Admin Portal",
                RedirectUris = { new Uri("https://localhost:5001/signin-oidc") },
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.GrantTypes.Password,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.ResponseTypes.Code,
                    OpenIddictConstants.Permissions.Scopes.Email,
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Roles,
                    OpenIddictConstants.Permissions.Prefixes.Scope + "openid",
                    OpenIddictConstants.Permissions.Prefixes.Scope + "api"
                }
            });
        }

     var adminRoleId = await dbContext.Roles .Where(r => r.Name == "Admin") .Select(r => r.Id) .FirstAsync(); 
     var alreadyAssigned = await dbContext.UserRoles .AnyAsync(ur => ur.UserId == adminUserId && ur.RoleId == adminRoleId && ur.IsDeleted == false); 
     if (!alreadyAssigned) { dbContext.UserRoles.Add(new UserRole { Id = Guid.NewGuid(), UserId = adminUserId, RoleId = adminRoleId, IsDeleted = false }); }

      await dbContext.SaveChangesAsync();
    }

  // public static async Task PopulateFakeUsersAsync(UserManager<ApplicationUser> userManager)
  // {
  //     var faker = new Faker<ApplicationUser>()
  //         .RuleFor(u => u.UserName, f => f.Internet.UserName())
  //         .RuleFor(u => u.Email, f => f.Internet.Email())
  //         .RuleFor(u => u.FirstName, f => f.Name.FirstName())
  //         .RuleFor(u => u.LastName, f => f.Name.LastName());

  //     for (int i = 0; i < NUMBER_OF_USERS; i++)
  //     {
  //         var user = faker.Generate();
  //         // Default password
  //         await userManager.CreateAsync(user, "Pass123!");
  //     }
  // }
}

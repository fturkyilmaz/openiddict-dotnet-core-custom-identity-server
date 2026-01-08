using Bogus;
using ShoppingProject.Core.ContributorAggregate;
using ShoppingProject.Core.UserAggregate;
using ShoppingProject.Core.OpenIdAggregate;
using ShoppingProject.Infrastructure.Common;

namespace ShoppingProject.Infrastructure.Data;

public static class SeedData
{
  public const int NUMBER_OF_CONTRIBUTORS = 27;
  public const int NUMBER_OF_USERS = 10; // kaç fake user istiyorsan

  public static readonly Contributor Contributor1 = new(ContributorName.From("Furkan"));
  public static readonly Contributor Contributor2 = new(ContributorName.From("Türkyılmaz"));

  public static async Task InitializeAsync(AppDbContext dbContext)
  {
    if (!await dbContext.Contributors.AnyAsync())
    {
      await PopulateTestDataAsync(dbContext);
    }

    if (!dbContext.Users.Any())
    {
      var (salt, hash) = PasswordHasher.Hash("Password123!");

      dbContext.Users.Add(new ApplicationUser
      {
        Id = Guid.NewGuid(),
        UserName = "admin",
        Email = "admin@test.com",
        DisplayName = "Administrator",
        PasswordSalt = salt,
        PasswordHash = hash
      });
    }

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

    await dbContext.SaveChangesAsync();
  }

  public static async Task PopulateTestDataAsync(AppDbContext dbContext)
  {
    dbContext.Contributors.AddRange([Contributor1, Contributor2]);


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

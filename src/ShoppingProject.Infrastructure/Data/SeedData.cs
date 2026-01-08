using Bogus;
using Microsoft.AspNetCore.Identity;
using ShoppingProject.Infrastructure.Identity;
using ShoppingProject.Core.ContributorAggregate;

namespace ShoppingProject.Infrastructure.Data;

public static class SeedData
{
    public const int NUMBER_OF_CONTRIBUTORS = 27;
    public const int NUMBER_OF_USERS = 10; // kaç fake user istiyorsan

    public static readonly Contributor Contributor1 = new(ContributorName.From("Ardalis"));
    public static readonly Contributor Contributor2 = new(ContributorName.From("Ilyana"));

    public static async Task InitializeAsync(AppDbContext dbContext)
    {
        if (!await dbContext.Contributors.AnyAsync())
        {
            await PopulateTestDataAsync(dbContext);
        }
    }

    public static async Task PopulateTestDataAsync(AppDbContext dbContext)
    {
        dbContext.Contributors.AddRange([Contributor1, Contributor2]);
        await dbContext.SaveChangesAsync();

        for (int i = 1; i <= NUMBER_OF_CONTRIBUTORS - 2; i++)
        {
            dbContext.Contributors.Add(new Contributor(ContributorName.From($"Contributor {i}")));
        }
        await dbContext.SaveChangesAsync();
    }

    public static async Task PopulateFakeUsersAsync(UserManager<ApplicationUser> userManager)
    {
        var faker = new Faker<ApplicationUser>()
            .RuleFor(u => u.UserName, f => f.Internet.UserName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName());

        for (int i = 0; i < NUMBER_OF_USERS; i++)
        {
            var user = faker.Generate();
            // Default password
            await userManager.CreateAsync(user, "Pass123!");
        }
    }
}

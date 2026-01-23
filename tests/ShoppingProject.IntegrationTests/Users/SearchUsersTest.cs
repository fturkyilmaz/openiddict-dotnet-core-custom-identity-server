using FluentAssertions;
using ShoppingProject.Core.UserAggregate;
using ShoppingProject.Infrastructure.Data;
using Xunit;
using System.Net;
using System.Net.Http.Headers;

namespace ShoppingProject.IntegrationTests.Users;

public class SearchUsersTests : IClassFixture<HttpClient>
{

    private readonly HttpClient _client;

    public SearchUsersTests(HttpClient fixture)
    {
        _client = fixture;
    }
    protected override async Task SeedAsync()
    {
        var admin = new User
        {
            Id = Guid.NewGuid(),
            UserName = "admin",
            Email = "admin@test.com",
            DisplayName = "Admin User",
            Status = UserStatus.Active,
            Claims =
            {
                new UserClaim("permission", "users.manage")
            }
        };

        var blocked = new User
        {
            Id = Guid.NewGuid(),
            UserName = "blocked.user",
            Email = "blocked@test.com",
            DisplayName = "Blocked User",
            Status = UserStatus.Blocked
        };

        var normal = new User
        {
            Id = Guid.NewGuid(),
            UserName = "john",
            Email = "john@test.com",
            DisplayName = "John Doe",
            Status = UserStatus.Active,
            Claims =
            {
                new UserClaim("department", "sales")
            }
        };

        Db.Users.AddRange(admin, blocked, normal);
        await Db.SaveChangesAsync();
    }

    private SearchUsersQueryHandler CreateHandler()
        => new(Db);

    // -------------------------
    // SEARCH
    // -------------------------

    [Fact]
    public async Task Search_By_Username_Should_Return_User()
    {
        var handler = CreateHandler();

        var result = await handler.Handle(
            new SearchUsersQuery("admin", null, false),
            default
        );

        result.Should().ContainSingle();
        result[0].UserName.Should().Be("admin");
    }

    [Fact]
    public async Task Search_By_Email_Should_Return_User()
    {
        var handler = CreateHandler();

        var result = await handler.Handle(
            new SearchUsersQuery("john@", null, false),
            default
        );

        result.Should().ContainSingle();
        result[0].Email.Should().Be("john@test.com");
    }

    [Fact]
    public async Task Search_By_DisplayName_Should_Return_User()
    {
        var handler = CreateHandler();

        var result = await handler.Handle(
            new SearchUsersQuery("Admin", null, false),
            default
        );

        result.Should().ContainSingle();
        result[0].DisplayName.Should().Contain("Admin");
    }

    // -------------------------
    // STATUS FILTER
    // -------------------------

    [Fact]
    public async Task Filter_By_Status_Active_Should_Return_Only_Active_Users()
    {
        var handler = CreateHandler();

        var result = await handler.Handle(
            new SearchUsersQuery(null, UserStatus.Active, false),
            default
        );

        result.Should().OnlyContain(u => u.Status == UserStatus.Active);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Filter_By_Status_Blocked_Should_Return_Only_Blocked_Users()
    {
        var handler = CreateHandler();

        var result = await handler.Handle(
            new SearchUsersQuery(null, UserStatus.Blocked, false),
            default
        );

        result.Should().ContainSingle();
        result[0].Status.Should().Be(UserStatus.Blocked);
    }

    // -------------------------
    // CLAIM SEARCH
    // -------------------------

    [Fact]
    public async Task Search_By_Claim_Should_Work_When_IncludeClaims_Is_True()
    {
        var handler = CreateHandler();

        var result = await handler.Handle(
            new SearchUsersQuery("users.manage", null, true),
            default
        );

        result.Should().ContainSingle();
        result[0].UserName.Should().Be("admin");
    }

    [Fact]
    public async Task Search_By_Claim_Should_Not_Work_When_IncludeClaims_Is_False()
    {
        var handler = CreateHandler();

        var result = await handler.Handle(
            new SearchUsersQuery("users.manage", null, false),
            default
        );

        result.Should().BeEmpty();
    }

    // -------------------------
    // COMBINED FILTERS
    // -------------------------

    [Fact]
    public async Task Search_And_Status_Filter_Should_Work_Together()
    {
        var handler = CreateHandler();

        var result = await handler.Handle(
            new SearchUsersQuery("john", UserStatus.Active, false),
            default
        );

        result.Should().ContainSingle();
        result[0].UserName.Should().Be("john");
    }
}

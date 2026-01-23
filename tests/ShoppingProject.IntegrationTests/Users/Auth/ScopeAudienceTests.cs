using System.Net;
using System.Net.Http.Headers;
using Xunit;

namespace ShoppingProject.IntegrationTests.Auth;

public class ScopeAudienceTests : IClassFixture<HttpClient>
{
    private readonly HttpClient _client;

    public ScopeAudienceTests(HttpClient fixture)
    {
        _client = fixture;
    }

    [Fact]
    public async Task Request_with_wrong_audience_should_return_401()
    {
        // Arrange
        var token = FakeJwtTokenGenerator.Generate(new()
        {
            Audience = "wrong-api",
            Scopes = ["api.users.read"]
        });

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Request_without_required_scope_should_return_403()
    {
        var token = FakeJwtTokenGenerator.Generate(new()
        {
            Audience = "shopping-api",
            Scopes = [] // eksik
        });

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/users");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Request_with_valid_scope_and_audience_should_return_200()
    {
        var token = FakeJwtTokenGenerator.Generate(new()
        {
            Audience = "shopping-api",
            Scopes = ["api.users.read"]
        });

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/users");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

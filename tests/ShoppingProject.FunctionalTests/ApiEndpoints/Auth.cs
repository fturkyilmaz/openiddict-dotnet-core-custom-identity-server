using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ShoppingProject.FunctionalTests.ApiEndpoints;

[Collection("Sequential")]
public class AuthorizationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthorizationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Token_ShouldReturnValidJwt()
    {
        var request = new { username = "testuser", password = "Pass123!" };

        var response = await _client.PostAsJsonAsync("/auth/token", request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<dynamic>();
        string token = (string)json!.access_token;

        // JWT parse
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        jwt.Should().NotBeNull();
        jwt.Claims.Should().Contain(c => c.Type == "sub");   // subject claim
        jwt.Claims.Should().Contain(c => c.Type == "name");  // username claim
    }

    [Fact]
    public async Task Refresh_ShouldReturnNewJwt()
    {
        var request = new { refresh_token = "validRefreshToken" };

        var response = await _client.PostAsJsonAsync("/auth/refresh", request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<dynamic>();
        string token = (string)json!.access_token;

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        jwt.Claims.Should().Contain(c => c.Type == "sub");
    }

    [Fact]
    public async Task Me_ShouldReturnProfile()
    {
        // Normalde Authorization header eklenir
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "validAccessToken");

        var response = await _client.GetAsync("/auth/me");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<dynamic>();
        ((string)json!.username).Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Revoke_ShouldReturnNoContent()
    {
        var request = new { refresh_token = "validRefreshToken" };

        var response = await _client.PostAsJsonAsync("/auth/revoke", request);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }
}

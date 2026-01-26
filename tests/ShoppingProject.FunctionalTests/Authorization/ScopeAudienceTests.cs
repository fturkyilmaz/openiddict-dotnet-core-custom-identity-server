using System.Net.Http.Headers;
using System.Text.Json;
using ShoppingProject.Web;

namespace ShoppingProject.FunctionalTests.Authorization;

[Collection("Sequential")]
public class ScopeAudienceTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task DebugClaims_ShouldReturnAllClaims()
    {
        // First get a valid token
        var loginRequest = new { username = "testuser", password = "Pass123!" };
        var loginResponse = await _client.PostAsJsonAsync("/connect/token", loginRequest);
        loginResponse.EnsureSuccessStatusCode();

        var loginJson = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        string token = loginJson.GetProperty("access_token").GetString()!;

        // Set the authorization header
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/protected/debug");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);
        
        result.TryGetProperty("claims", out var claims).ShouldBeTrue();
        
        // Check that basic claims exist
        var claimsArray = claims.EnumerateArray();
        var scopeClaims = claimsArray.Where(c => c.GetProperty("Type").GetString() == "scope").ToList();
        scopeClaims.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_ShouldReturn401()
    {
        // Act
        var response = await _client.GetAsync("/api/protected/users");

        // Assert
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithValidToken_ShouldReturn200()
    {
        // First get a valid token
        var loginRequest = new { username = "testuser", password = "Pass123!" };
        var loginResponse = await _client.PostAsJsonAsync("/connect/token", loginRequest);
        loginResponse.EnsureSuccessStatusCode();

        var loginJson = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        string token = loginJson.GetProperty("access_token").GetString()!;

        // Set the authorization header
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/protected/basic");

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
    }
}
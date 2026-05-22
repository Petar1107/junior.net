using System.Net;
using System.Net.Http.Json;
using AbySalto.Junior.Application.DTOs.Auth;
using AbySalto.Junior.IntegrationTests.Fixtures;
using FluentAssertions;

namespace AbySalto.Junior.IntegrationTests.Endpoints;

[Collection(nameof(IntegrationTestCollection))]
public class AuthEndpointsTests
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(AbySaltoApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithSeededAdmin_ReturnsToken()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest
            {
                Email = TestCredentials.AdminEmail,
                Password = TestCredentials.AdminPassword,
            },
            cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken);
        auth.Should().NotBeNull();
        auth.AccessToken.Should().NotBeNullOrWhiteSpace();
        auth.ExpiresAtUtc.Should().BeAfter(DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest
            {
                Email = TestCredentials.AdminEmail,
                Password = "WrongPassword1!",
            },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

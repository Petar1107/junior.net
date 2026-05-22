using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AbySalto.Junior.Application.DTOs.Auth;
using FluentAssertions;

namespace AbySalto.Junior.IntegrationTests.Fixtures;

internal static class IntegrationTestHelper
{
    public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    };
    public static async Task<string> LoginAsAdminAsync(HttpClient client, CancellationToken cancellationToken = default)
    {
        cancellationToken = cancellationToken == default
            ? TestContext.Current.CancellationToken
            : cancellationToken;

        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest
            {
                Email = TestCredentials.AdminEmail,
                Password = TestCredentials.AdminPassword,
            },
            cancellationToken);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken);
        auth.Should().NotBeNull();
        auth.AccessToken.Should().NotBeNullOrWhiteSpace();

        return auth.AccessToken;
    }

    public static void SetBearerToken(HttpClient client, string accessToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }
}

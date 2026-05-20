namespace AbySalto.Junior.Infrastructure.Auth;

public class AuthTokenResult
{
    public string AccessToken { get; init; } = string.Empty;

    public DateTimeOffset ExpiresAtUtc { get; init; }
}

namespace AbySalto.Junior.Application.Models.Auth;

public class AuthResponse
{
    public string AccessToken { get; init; } = string.Empty;

    public DateTimeOffset ExpiresAtUtc { get; init; }
}

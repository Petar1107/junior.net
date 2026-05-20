using AbySalto.Junior.Domain.Entities.Identity;

namespace AbySalto.Junior.Infrastructure.Auth;

public interface IJwtTokenService
{
    AuthTokenResult GenerateToken(ApplicationUser user, IEnumerable<string> roles);
}

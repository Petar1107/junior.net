using Microsoft.AspNetCore.Identity;

namespace AbySalto.Junior.Domain.Entities.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = String.Empty;
    public string LastName { get; set; } = String.Empty;
    // Add orders collection after.
}
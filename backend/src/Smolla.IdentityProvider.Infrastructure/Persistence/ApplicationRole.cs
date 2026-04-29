using Microsoft.AspNetCore.Identity;

namespace Smolla.IdentityProvider.Infrastructure.Persistence;

public sealed class ApplicationRole : IdentityRole<Guid>
{
    public string? Description { get; set; }
}

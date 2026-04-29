using Microsoft.AspNetCore.Identity;

namespace Smolla.IdentityProvider.Infrastructure.Persistence;

/// <summary>
/// Persistence-level user record used by ASP.NET Core Identity.
/// Mapped to the domain <see cref="Domain.Entities.User"/> by the repository.
/// </summary>
public sealed class ApplicationUser : IdentityUser<Guid>
{
    public required string DisplayName { get; set; }

    public bool IsDisabled { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? LastLoginAt { get; set; }
}

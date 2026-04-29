namespace Smolla.IdentityProvider.Domain.Entities;

/// <summary>
/// Domain representation of a user. Pure POCO with no persistence concerns.
/// Identity-specific fields (password hash, security stamps) live on the
/// infrastructure-level <c>ApplicationUser</c>.
/// </summary>
public sealed class User
{
    public required Guid Id { get; init; }

    public required string Email { get; set; }

    public required string DisplayName { get; set; }

    public bool IsDisabled { get; set; }

    public bool IsEmailVerified { get; set; }

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? LastLoginAt { get; set; }

    public IReadOnlyCollection<string> Roles { get; init; } = [];
}

using Smolla.IdentityProvider.Domain.Entities;

namespace Smolla.IdentityProvider.Infrastructure.Persistence.Mappings;

internal static class UserMappings
{
    public static User ToDomain(ApplicationUser user, IReadOnlyCollection<string> roles)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(roles);

        return new User
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            DisplayName = user.DisplayName,
            IsDisabled = user.IsDisabled,
            IsEmailVerified = user.EmailConfirmed,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            Roles = roles
        };
    }
}

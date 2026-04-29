using Smolla.IdentityProvider.Domain.Entities;

namespace Smolla.IdentityProvider.Application.Users;

public sealed record UserResult
{
    public required Guid Id { get; init; }

    public required string Email { get; init; }

    public required string DisplayName { get; init; }

    public required bool IsDisabled { get; init; }

    public required bool IsEmailVerified { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? LastLoginAt { get; init; }

    public IReadOnlyCollection<string> Roles { get; init; } = [];

    public static UserResult FromDomain(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return new UserResult
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            IsDisabled = user.IsDisabled,
            IsEmailVerified = user.IsEmailVerified,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            Roles = user.Roles
        };
    }
}

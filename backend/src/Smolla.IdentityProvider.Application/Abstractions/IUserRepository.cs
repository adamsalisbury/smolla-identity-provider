using Smolla.IdentityProvider.Application.Users;
using Smolla.IdentityProvider.Domain.Entities;

namespace Smolla.IdentityProvider.Application.Abstractions;

/// <summary>
/// Persistence-level access to <see cref="User"/> aggregates. Returns domain
/// entities, never EF Core or Identity types.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Finds a user by its primary id.
    /// </summary>
    Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a user by email address. Email comparison is case-insensitive.
    /// </summary>
    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches users matching the given query.
    /// </summary>
    Task<IReadOnlyList<User>> SearchAsync(UserSearchQuery query, CancellationToken cancellationToken = default);
}

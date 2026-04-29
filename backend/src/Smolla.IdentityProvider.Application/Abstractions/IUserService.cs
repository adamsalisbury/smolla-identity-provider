using Smolla.IdentityProvider.Application.Common;
using Smolla.IdentityProvider.Application.Users;

namespace Smolla.IdentityProvider.Application.Abstractions;

/// <summary>
/// High-level user management operations consumed by API controllers.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="request">The user details.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created user, or the validation/identity errors that prevented creation.</returns>
    Task<OperationResult<UserResult>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by id.
    /// </summary>
    Task<UserResult?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a paged set of users matching the given query.
    /// </summary>
    Task<IReadOnlyList<UserResult>> SearchAsync(UserSearchQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a user's mutable fields.
    /// </summary>
    Task<OperationResult<UserResult>> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a user as disabled. Disabled users cannot authenticate.
    /// </summary>
    Task<OperationResult<UserResult>> DisableAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Replaces a user's role assignments.
    /// </summary>
    Task<OperationResult<UserResult>> AssignRolesAsync(Guid id, IEnumerable<string> roles, CancellationToken cancellationToken = default);
}

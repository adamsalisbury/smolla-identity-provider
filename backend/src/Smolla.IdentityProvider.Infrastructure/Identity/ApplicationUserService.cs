using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Smolla.IdentityProvider.Application.Abstractions;
using Smolla.IdentityProvider.Application.Common;
using Smolla.IdentityProvider.Application.Users;
using Smolla.IdentityProvider.Infrastructure.Persistence;

namespace Smolla.IdentityProvider.Infrastructure.Identity;

internal sealed class ApplicationUserService(
    UserManager<ApplicationUser> userManager,
    IUserRepository userRepository,
    IValidator<CreateUserRequest> createUserValidator,
    ILogger<ApplicationUserService> logger) : IUserService
{
    /// <inheritdoc/>
    public async Task<OperationResult<UserResult>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        FluentValidation.Results.ValidationResult validation = await createUserValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return OperationResult<UserResult>.Failure(validation.Errors.Select(e => e.ErrorMessage));
        }

        ApplicationUser entity = new()
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.DisplayName,
            CreatedAt = DateTimeOffset.UtcNow
        };

        IdentityResult creation = await userManager.CreateAsync(entity, request.Password);
        if (!creation.Succeeded)
        {
            logger.LogWarning("Failed to create user {Email}: {Errors}",
                request.Email,
                string.Join(", ", creation.Errors.Select(e => e.Description)));

            return OperationResult<UserResult>.Failure(creation.Errors.Select(e => e.Description));
        }

        if (request.Roles.Count > 0)
        {
            IdentityResult roleAssignment = await userManager.AddToRolesAsync(entity, request.Roles);
            if (!roleAssignment.Succeeded)
            {
                return OperationResult<UserResult>.Failure(roleAssignment.Errors.Select(e => e.Description));
            }
        }

        Domain.Entities.User? created = await userRepository.FindByIdAsync(entity.Id, cancellationToken);
        return created is null
            ? OperationResult<UserResult>.Failure("User created but could not be re-loaded.")
            : OperationResult<UserResult>.Success(UserResult.FromDomain(created));
    }

    /// <inheritdoc/>
    public async Task<UserResult?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Domain.Entities.User? user = await userRepository.FindByIdAsync(id, cancellationToken);
        return user is null ? null : UserResult.FromDomain(user);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<UserResult>> SearchAsync(UserSearchQuery query, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Domain.Entities.User> users = await userRepository.SearchAsync(query, cancellationToken);
        return [.. users.Select(UserResult.FromDomain)];
    }

    /// <inheritdoc/>
    public async Task<OperationResult<UserResult>> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ApplicationUser? entity = await userManager.FindByIdAsync(id.ToString());
        if (entity is null)
        {
            return OperationResult<UserResult>.Failure("User not found.");
        }

        if (request.DisplayName is { } displayName)
        {
            entity.DisplayName = displayName;
        }

        if (request.IsDisabled is { } disabled)
        {
            entity.IsDisabled = disabled;
        }

        IdentityResult update = await userManager.UpdateAsync(entity);
        if (!update.Succeeded)
        {
            return OperationResult<UserResult>.Failure(update.Errors.Select(e => e.Description));
        }

        Domain.Entities.User? reloaded = await userRepository.FindByIdAsync(entity.Id, cancellationToken);
        return reloaded is null
            ? OperationResult<UserResult>.Failure("User updated but could not be re-loaded.")
            : OperationResult<UserResult>.Success(UserResult.FromDomain(reloaded));
    }

    /// <inheritdoc/>
    public Task<OperationResult<UserResult>> DisableAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return UpdateAsync(id, new UpdateUserRequest { IsDisabled = true }, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<OperationResult<UserResult>> AssignRolesAsync(Guid id, IEnumerable<string> roles, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(roles);

        ApplicationUser? entity = await userManager.FindByIdAsync(id.ToString());
        if (entity is null)
        {
            return OperationResult<UserResult>.Failure("User not found.");
        }

        IList<string> existing = await userManager.GetRolesAsync(entity);
        string[] desired = [.. roles];

        IEnumerable<string> toRemove = existing.Except(desired, StringComparer.OrdinalIgnoreCase);
        IEnumerable<string> toAdd = desired.Except(existing, StringComparer.OrdinalIgnoreCase);

        IdentityResult removeResult = await userManager.RemoveFromRolesAsync(entity, toRemove);
        if (!removeResult.Succeeded)
        {
            return OperationResult<UserResult>.Failure(removeResult.Errors.Select(e => e.Description));
        }

        IdentityResult addResult = await userManager.AddToRolesAsync(entity, toAdd);
        if (!addResult.Succeeded)
        {
            return OperationResult<UserResult>.Failure(addResult.Errors.Select(e => e.Description));
        }

        Domain.Entities.User? reloaded = await userRepository.FindByIdAsync(entity.Id, cancellationToken);
        return reloaded is null
            ? OperationResult<UserResult>.Failure("User updated but could not be re-loaded.")
            : OperationResult<UserResult>.Success(UserResult.FromDomain(reloaded));
    }
}

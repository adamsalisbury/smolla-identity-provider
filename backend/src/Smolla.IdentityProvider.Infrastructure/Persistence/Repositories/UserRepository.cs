using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Smolla.IdentityProvider.Application.Abstractions;
using Smolla.IdentityProvider.Application.Users;
using Smolla.IdentityProvider.Domain.Entities;
using Smolla.IdentityProvider.Infrastructure.Persistence.Mappings;

namespace Smolla.IdentityProvider.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(
    AppDbContext dbContext,
    UserManager<ApplicationUser> userManager) : IUserRepository
{
    /// <inheritdoc/>
    public async Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ApplicationUser? entity = await dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        IList<string> roles = await userManager.GetRolesAsync(entity);
        return UserMappings.ToDomain(entity, [.. roles]);
    }

    /// <inheritdoc/>
    public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        string normalised = email.Trim().ToUpperInvariant();
        ApplicationUser? entity = await dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.NormalizedEmail == normalised, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        IList<string> roles = await userManager.GetRolesAsync(entity);
        return UserMappings.ToDomain(entity, [.. roles]);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<User>> SearchAsync(UserSearchQuery query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        IQueryable<ApplicationUser> q = dbContext.Users.AsNoTracking();

        if (query.IncludeDisabled is not true)
        {
            q = q.Where(u => !u.IsDisabled);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string term = query.Search.Trim();
            q = q.Where(u =>
                EF.Functions.Like(u.Email!, $"%{term}%") ||
                EF.Functions.Like(u.DisplayName, $"%{term}%"));
        }

        List<ApplicationUser> page = await q
            .OrderBy(u => u.DisplayName)
            .Skip(query.Skip)
            .Take(query.Take)
            .ToListAsync(cancellationToken);

        List<User> result = new(page.Count);
        foreach (ApplicationUser entity in page)
        {
            IList<string> roles = await userManager.GetRolesAsync(entity);
            result.Add(UserMappings.ToDomain(entity, [.. roles]));
        }

        return result;
    }
}

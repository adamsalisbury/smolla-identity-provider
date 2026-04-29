using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using Smolla.IdentityProvider.Domain.Entities;
using Smolla.IdentityProvider.Infrastructure.Persistence;

namespace Smolla.IdentityProvider.Infrastructure.Seeding;

/// <summary>
/// Bootstraps the database with built-in roles and an administrator user
/// plus an admin OAuth client on first run. Idempotent — safe to invoke on
/// every startup.
/// </summary>
public sealed class IdentitySeeder(
    AppDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IOpenIddictApplicationManager applicationManager,
    IOptions<SeedOptions> options,
    ILogger<IdentitySeeder> logger)
{
    private readonly SeedOptions _options = options.Value;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);

        await EnsureRolesAsync();
        await EnsureAdminUserAsync(cancellationToken);
        await EnsureAdminClientAsync(cancellationToken);
    }

    private async Task EnsureRolesAsync()
    {
        foreach (string roleName in new[] { Roles.Administrator, Roles.User })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                ApplicationRole role = new()
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    Description = $"{roleName} role"
                };
                IdentityResult result = await roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    logger.LogError("Failed to create role {Role}: {Errors}",
                        roleName,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    private async Task EnsureAdminUserAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.AdminEmail) || string.IsNullOrWhiteSpace(_options.AdminPassword))
        {
            logger.LogInformation("Admin seed skipped — Seed:AdminEmail and Seed:AdminPassword are not set.");
            return;
        }

        ApplicationUser? existing = await userManager.FindByEmailAsync(_options.AdminEmail);
        if (existing is not null)
        {
            return;
        }

        ApplicationUser admin = new()
        {
            Id = Guid.NewGuid(),
            UserName = _options.AdminEmail,
            Email = _options.AdminEmail,
            EmailConfirmed = true,
            DisplayName = _options.AdminDisplayName ?? "Administrator",
            CreatedAt = DateTimeOffset.UtcNow
        };

        IdentityResult creation = await userManager.CreateAsync(admin, _options.AdminPassword);
        if (!creation.Succeeded)
        {
            logger.LogError("Failed to seed admin user: {Errors}",
                string.Join(", ", creation.Errors.Select(e => e.Description)));
            return;
        }

        await userManager.AddToRoleAsync(admin, Roles.Administrator);
        logger.LogInformation("Seeded admin user {Email}.", _options.AdminEmail);
    }

    private async Task EnsureAdminClientAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.AdminClientId))
        {
            logger.LogInformation("Admin client seed skipped — Seed:AdminClientId is not set.");
            return;
        }

        if (await applicationManager.FindByClientIdAsync(_options.AdminClientId, cancellationToken) is not null)
        {
            return;
        }

        OpenIddictApplicationDescriptor descriptor = new()
        {
            ClientId = _options.AdminClientId,
            ClientSecret = string.IsNullOrWhiteSpace(_options.AdminClientSecret) ? null : _options.AdminClientSecret,
            DisplayName = _options.AdminClientDisplayName ?? "Smolla Identity Admin",
            ClientType = string.IsNullOrWhiteSpace(_options.AdminClientSecret)
                ? OpenIddictConstants.ClientTypes.Public
                : OpenIddictConstants.ClientTypes.Confidential,
            ConsentType = OpenIddictConstants.ConsentTypes.Implicit
        };

        descriptor.Permissions.UnionWith(new[]
        {
            OpenIddictConstants.Permissions.Endpoints.Authorization,
            OpenIddictConstants.Permissions.Endpoints.Token,
            OpenIddictConstants.Permissions.Endpoints.EndSession,
            OpenIddictConstants.Permissions.Endpoints.Revocation,
            OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
            OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
            OpenIddictConstants.Permissions.ResponseTypes.Code,
            OpenIddictConstants.Permissions.Scopes.Email,
            OpenIddictConstants.Permissions.Scopes.Profile,
            OpenIddictConstants.Permissions.Scopes.Roles
        });

        descriptor.Requirements.Add(OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange);

        foreach (string uri in _options.AdminClientRedirectUris)
        {
            descriptor.RedirectUris.Add(new Uri(uri));
        }

        foreach (string uri in _options.AdminClientPostLogoutRedirectUris)
        {
            descriptor.PostLogoutRedirectUris.Add(new Uri(uri));
        }

        await applicationManager.CreateAsync(descriptor, cancellationToken);
        logger.LogInformation("Seeded admin OAuth client {ClientId}.", _options.AdminClientId);
    }
}

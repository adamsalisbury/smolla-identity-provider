using System.Collections.Immutable;
using OpenIddict.Abstractions;
using Smolla.IdentityProvider.Application.Abstractions;
using Smolla.IdentityProvider.Application.Clients;
using Smolla.IdentityProvider.Domain.Entities;

namespace Smolla.IdentityProvider.Infrastructure.Identity;

internal sealed class OpenIddictClientService(IOpenIddictApplicationManager applicationManager) : IClientService
{
    /// <inheritdoc/>
    public async Task<IReadOnlyList<ClientResult>> ListAsync(CancellationToken cancellationToken = default)
    {
        List<ClientResult> results = [];
        await foreach (object descriptor in applicationManager.ListAsync(count: null, offset: null, cancellationToken))
        {
            ClientResult? mapped = await MapAsync(descriptor, cancellationToken);
            if (mapped is not null)
            {
                results.Add(mapped);
            }
        }

        return results;
    }

    /// <inheritdoc/>
    public async Task<ClientResult?> FindByClientIdAsync(string clientId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clientId);

        object? descriptor = await applicationManager.FindByClientIdAsync(clientId, cancellationToken);
        return descriptor is null ? null : await MapAsync(descriptor, cancellationToken);
    }

    private async Task<ClientResult?> MapAsync(object descriptor, CancellationToken cancellationToken)
    {
        string? id = await applicationManager.GetIdAsync(descriptor, cancellationToken);
        string? clientId = await applicationManager.GetClientIdAsync(descriptor, cancellationToken);
        string? displayName = await applicationManager.GetDisplayNameAsync(descriptor, cancellationToken);
        string? clientType = await applicationManager.GetClientTypeAsync(descriptor, cancellationToken);
        ImmutableArray<string> redirectUris = await applicationManager.GetRedirectUrisAsync(descriptor, cancellationToken);
        ImmutableArray<string> postLogoutUris = await applicationManager.GetPostLogoutRedirectUrisAsync(descriptor, cancellationToken);
        ImmutableArray<string> permissions = await applicationManager.GetPermissionsAsync(descriptor, cancellationToken);
        ImmutableArray<string> requirements = await applicationManager.GetRequirementsAsync(descriptor, cancellationToken);
        ImmutableArray<string> consent = await applicationManager.GetConsentTypeAsync(descriptor, cancellationToken) is { Length: > 0 } c
            ? [c]
            : [];

        if (id is null || clientId is null)
        {
            return null;
        }

        return new ClientResult
        {
            Id = Guid.TryParse(id, out Guid parsed) ? parsed : Guid.Empty,
            ClientId = clientId,
            DisplayName = displayName ?? clientId,
            ClientType = string.Equals(clientType, OpenIddictConstants.ClientTypes.Confidential, StringComparison.OrdinalIgnoreCase)
                ? OAuthClientType.Confidential
                : OAuthClientType.Public,
            RedirectUris = [.. redirectUris],
            PostLogoutRedirectUris = [.. postLogoutUris],
            Permissions = [.. permissions],
            RequirePkce = requirements.Contains(OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange),
            RequireConsent = consent.Contains(OpenIddictConstants.ConsentTypes.Explicit)
        };
    }
}

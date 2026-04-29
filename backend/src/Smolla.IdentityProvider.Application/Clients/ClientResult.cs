using Smolla.IdentityProvider.Domain.Entities;

namespace Smolla.IdentityProvider.Application.Clients;

public sealed record ClientResult
{
    public required Guid Id { get; init; }

    public required string ClientId { get; init; }

    public required string DisplayName { get; init; }

    public required OAuthClientType ClientType { get; init; }

    public required IReadOnlyCollection<string> RedirectUris { get; init; }

    public required IReadOnlyCollection<string> PostLogoutRedirectUris { get; init; }

    public required IReadOnlyCollection<string> Permissions { get; init; }

    public required bool RequirePkce { get; init; }

    public required bool RequireConsent { get; init; }

    public static ClientResult FromDomain(OAuthClient client)
    {
        ArgumentNullException.ThrowIfNull(client);

        return new ClientResult
        {
            Id = client.Id,
            ClientId = client.ClientId,
            DisplayName = client.DisplayName,
            ClientType = client.ClientType,
            RedirectUris = client.RedirectUris,
            PostLogoutRedirectUris = client.PostLogoutRedirectUris,
            Permissions = client.Permissions,
            RequirePkce = client.RequirePkce,
            RequireConsent = client.RequireConsent
        };
    }
}

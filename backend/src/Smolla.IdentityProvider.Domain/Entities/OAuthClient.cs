namespace Smolla.IdentityProvider.Domain.Entities;

/// <summary>
/// Domain representation of a registered OAuth 2.0 / OIDC client. Mirrors a
/// subset of the OpenIddict application record, exposed to the application
/// layer so business logic does not depend on OpenIddict types.
/// </summary>
public sealed class OAuthClient
{
    public required Guid Id { get; init; }

    public required string ClientId { get; set; }

    public required string DisplayName { get; set; }

    public required OAuthClientType ClientType { get; set; }

    public IReadOnlyCollection<string> RedirectUris { get; set; } = [];

    public IReadOnlyCollection<string> PostLogoutRedirectUris { get; set; } = [];

    public IReadOnlyCollection<string> Permissions { get; set; } = [];

    public bool RequirePkce { get; set; } = true;

    public bool RequireConsent { get; set; }

    public DateTimeOffset CreatedAt { get; init; }
}

public enum OAuthClientType
{
    Public = 0,
    Confidential = 1
}

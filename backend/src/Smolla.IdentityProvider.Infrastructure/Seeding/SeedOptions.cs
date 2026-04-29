namespace Smolla.IdentityProvider.Infrastructure.Seeding;

/// <summary>
/// Bootstrap data injected on first run. Sourced from environment variables
/// or appsettings; never hard-coded. Empty values disable seeding for the
/// associated artefact.
/// </summary>
public sealed class SeedOptions
{
    public const string SectionName = "Seed";

    public string? AdminEmail { get; set; }

    public string? AdminPassword { get; set; }

    public string? AdminDisplayName { get; set; } = "Administrator";

    public string? AdminClientId { get; set; }

    public string? AdminClientSecret { get; set; }

    public string? AdminClientDisplayName { get; set; } = "Smolla Identity Admin";

    public IList<string> AdminClientRedirectUris { get; set; } = [];

    public IList<string> AdminClientPostLogoutRedirectUris { get; set; } = [];
}

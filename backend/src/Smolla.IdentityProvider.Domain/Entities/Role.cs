namespace Smolla.IdentityProvider.Domain.Entities;

/// <summary>
/// Domain role. Roles are a flat list of named grants attached to users.
/// </summary>
public sealed class Role
{
    public required Guid Id { get; init; }

    public required string Name { get; set; }

    public string? Description { get; set; }
}

/// <summary>
/// Built-in role names. Used by the seeder and by application policies.
/// </summary>
public static class Roles
{
    public const string Administrator = "Administrator";
    public const string User = "User";
}

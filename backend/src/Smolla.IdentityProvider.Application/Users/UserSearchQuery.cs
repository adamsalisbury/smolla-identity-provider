namespace Smolla.IdentityProvider.Application.Users;

public sealed record UserSearchQuery
{
    public string? Search { get; init; }

    public bool? IncludeDisabled { get; init; }

    public int Skip { get; init; }

    public int Take { get; init; } = 50;
}

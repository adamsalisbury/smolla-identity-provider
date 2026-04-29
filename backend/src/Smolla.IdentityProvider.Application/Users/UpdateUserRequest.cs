namespace Smolla.IdentityProvider.Application.Users;

public sealed record UpdateUserRequest
{
    public string? DisplayName { get; init; }

    public bool? IsDisabled { get; init; }
}

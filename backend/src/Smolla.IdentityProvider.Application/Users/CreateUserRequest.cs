namespace Smolla.IdentityProvider.Application.Users;

public sealed record CreateUserRequest
{
    public required string Email { get; init; }

    public required string Password { get; init; }

    public required string DisplayName { get; init; }

    public IReadOnlyCollection<string> Roles { get; init; } = [];
}

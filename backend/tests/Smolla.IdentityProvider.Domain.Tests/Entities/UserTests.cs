using FluentAssertions;
using Smolla.IdentityProvider.Domain.Entities;

namespace Smolla.IdentityProvider.Domain.Tests.Entities;

public sealed class UserTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void New_user_should_default_to_no_roles_and_not_disabled()
    {
        User user = new()
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            DisplayName = "Test User",
            CreatedAt = DateTimeOffset.UtcNow
        };

        user.Roles.Should().BeEmpty();
        user.IsDisabled.Should().BeFalse();
        user.IsEmailVerified.Should().BeFalse();
        user.LastLoginAt.Should().BeNull();
    }
}

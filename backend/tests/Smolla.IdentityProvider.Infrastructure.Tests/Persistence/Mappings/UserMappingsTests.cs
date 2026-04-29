using FluentAssertions;
using Smolla.IdentityProvider.Domain.Entities;
using Smolla.IdentityProvider.Infrastructure.Persistence;

namespace Smolla.IdentityProvider.Infrastructure.Tests.Persistence.Mappings;

public sealed class UserMappingsTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void ToDomain_should_project_application_user_with_roles()
    {
        ApplicationUser entity = new()
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            DisplayName = "Sample",
            EmailConfirmed = true,
            IsDisabled = false,
            CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        string[] roles = [Roles.Administrator];

        User domain = Smolla.IdentityProvider.Infrastructure.Persistence.Mappings.UserMappings.ToDomain(entity, roles);

        domain.Id.Should().Be(entity.Id);
        domain.Email.Should().Be(entity.Email);
        domain.DisplayName.Should().Be(entity.DisplayName);
        domain.IsEmailVerified.Should().BeTrue();
        domain.IsDisabled.Should().BeFalse();
        domain.Roles.Should().ContainSingle().Which.Should().Be(Roles.Administrator);
    }
}

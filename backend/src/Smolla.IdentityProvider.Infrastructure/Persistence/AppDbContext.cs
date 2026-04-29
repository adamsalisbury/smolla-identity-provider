using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Smolla.IdentityProvider.Infrastructure.Persistence;

/// <summary>
/// Single EF Core context backing both ASP.NET Core Identity and OpenIddict.
/// OpenIddict stores are added via <c>UseOpenIddict()</c> at registration time.
/// </summary>
public sealed class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(b =>
        {
            b.Property(u => u.DisplayName).HasMaxLength(128).IsRequired();
        });

        builder.Entity<ApplicationRole>(b =>
        {
            b.Property(r => r.Description).HasMaxLength(256);
        });
    }
}

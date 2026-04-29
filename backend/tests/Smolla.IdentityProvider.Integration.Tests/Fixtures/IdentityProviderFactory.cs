using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Smolla.IdentityProvider.Infrastructure.Persistence;
using Testcontainers.MsSql;

namespace Smolla.IdentityProvider.Integration.Tests.Fixtures;

/// <summary>
/// Boots the identity provider host for end-to-end HTTP tests. A SQL Server
/// 2022 container is provisioned per test class via Testcontainers.
/// </summary>
public sealed class IdentityProviderFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _database = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public Task InitializeAsync()
    {
        return _database.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _database.DisposeAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(_database.GetConnectionString());
                options.UseOpenIddict();
            });
        });
    }
}

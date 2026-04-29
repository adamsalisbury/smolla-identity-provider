using Serilog;
using Smolla.IdentityProvider.Application.DependencyInjection;
using Smolla.IdentityProvider.Host.Configuration;
using Smolla.IdentityProvider.Infrastructure.DependencyInjection;
using Smolla.IdentityProvider.Infrastructure.Seeding;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, _, loggerConfig) =>
{
    loggerConfig
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddOpenIddictServer(builder.Configuration);
builder.Services.AddIdentityProviderApi();

WebApplication app = builder.Build();

app.UseSerilogRequestLogging();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Seed:RunOnStartup"))
{
    using IServiceScope scope = app.Services.CreateScope();
    IdentitySeeder seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
    await seeder.RunAsync();
}

app.Run();

/// <summary>
/// Marker type so <c>WebApplicationFactory&lt;Program&gt;</c> can boot the host
/// from the integration tests project.
/// </summary>
public partial class Program;

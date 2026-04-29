using Smolla.IdentityProvider.Api.Controllers;

namespace Smolla.IdentityProvider.Host.Configuration;

/// <summary>
/// Registers the controllers from the Api assembly with the MVC pipeline.
/// </summary>
public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityProviderApi(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddControllers()
            .AddApplicationPart(typeof(HealthController).Assembly);

        services.AddOpenApi();

        return services;
    }
}
